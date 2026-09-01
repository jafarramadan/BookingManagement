using BookingManagement.Common.Exceptions;
using BookingManagement.Common.Enums;
using BookingManagement.Tests.TestSupport;
using static BookingManagement.Tests.TestSupport.BookingTestData;

namespace BookingManagement.Tests.BL
{
    public class BookingServiceCreateTests
    {
        [Fact]
        public async Task CreateAsync_WithValidRequest_StoresAnActiveBooking()
        {
            var context = new BookingServiceContext();

            var booking = await context.Service.CreateAsync(CreateRequest(Utc(10), Utc(11)));

            Assert.NotEqual(Guid.Empty, booking.Id);
            Assert.Equal(ResourceId, booking.ResourceId);
            Assert.Equal(UserId, booking.UserId);
            Assert.Equal(Utc(10), booking.StartDateTime);
            Assert.Equal(Utc(11), booking.EndDateTime);
            Assert.Equal("Active", booking.Status);
            Assert.Null(booking.CancelledAt);
            Assert.Equal(DateTimeKind.Utc, booking.CreatedAt.Kind);
            Assert.Single(context.Bookings);
            Assert.Equal(1, context.SaveChangesCount);
        }

        [Fact]
        public async Task CreateAsync_WithValidRequest_WritesABookingCreatedAuditLog()
        {
            var context = new BookingServiceContext();

            var booking = await context.Service.CreateAsync(CreateRequest(Utc(10), Utc(11)));

            var auditLog = Assert.Single(context.AuditLogs);
            Assert.Equal(AuditEventType.BookingCreated, auditLog.EventType);
            Assert.Equal(booking.Id, auditLog.BookingId);
            Assert.Equal(booking.CreatedAt, auditLog.OccurredAt);
            Assert.Equal(ResourceId, auditLog.ResourceId);
            Assert.Equal(UserId, auditLog.UserId);
            Assert.NotEqual(Guid.Empty, auditLog.Id);
        }

        [Fact]
        public async Task CreateAsync_WritesTheBookingAndTheAuditLogInOneSaveChanges()
        {
            var context = new BookingServiceContext();

            await context.Service.CreateAsync(CreateRequest(Utc(10), Utc(11)));

            Assert.Single(context.Bookings);
            Assert.Single(context.AuditLogs);
            Assert.Equal(1, context.SaveChangesCount);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task CreateAsync_WithoutResourceId_IsRejected(string? resourceId)
        {
            var context = new BookingServiceContext();
            var request = CreateRequest(Utc(10), Utc(11), resourceId: resourceId);

            await Assert.ThrowsAsync<BusinessValidationException>(() => context.Service.CreateAsync(request));
            Assert.Empty(context.Bookings);
            Assert.Empty(context.AuditLogs);
            Assert.Equal(0, context.SaveChangesCount);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task CreateAsync_WithoutUserId_IsRejected(string? userId)
        {
            var context = new BookingServiceContext();
            var request = CreateRequest(Utc(10), Utc(11), userId: userId);

            await Assert.ThrowsAsync<BusinessValidationException>(() => context.Service.CreateAsync(request));
            Assert.Empty(context.Bookings);
            Assert.Empty(context.AuditLogs);
        }

        [Fact]
        public async Task CreateAsync_WhenStartEqualsEnd_IsRejected()
        {
            var context = new BookingServiceContext();

            await Assert.ThrowsAsync<BusinessValidationException>(
                () => context.Service.CreateAsync(CreateRequest(Utc(10), Utc(10))));
        }

        [Fact]
        public async Task CreateAsync_WhenStartIsAfterEnd_IsRejected()
        {
            var context = new BookingServiceContext();

            await Assert.ThrowsAsync<BusinessValidationException>(
                () => context.Service.CreateAsync(CreateRequest(Utc(11), Utc(10))));
        }

        [Theory]
        [InlineData(DateTimeKind.Unspecified)]
        [InlineData(DateTimeKind.Local)]
        public async Task CreateAsync_WhenDatesAreNotUtc_IsRejected(DateTimeKind kind)
        {
            var context = new BookingServiceContext();
            var request = CreateRequest(
                DateTime.SpecifyKind(Utc(10), kind),
                DateTime.SpecifyKind(Utc(11), kind));

            await Assert.ThrowsAsync<BusinessValidationException>(() => context.Service.CreateAsync(request));
        }

        [Fact]
        public async Task CreateAsync_WhenAnActiveBookingOverlaps_IsRejected()
        {
            var context = new BookingServiceContext(ActiveBooking(Utc(10), Utc(11)));

            await Assert.ThrowsAsync<ConflictException>(
                () => context.Service.CreateAsync(CreateRequest(Utc(10, 30), Utc(11, 30))));

            Assert.Single(context.Bookings);
            Assert.Empty(context.AuditLogs);
            Assert.Equal(0, context.SaveChangesCount);
        }

        [Fact]
        public async Task CreateAsync_WhenTheRequestedSlotContainsAnActiveBooking_IsRejected()
        {
            var context = new BookingServiceContext(ActiveBooking(Utc(10, 15), Utc(10, 45)));

            await Assert.ThrowsAsync<ConflictException>(
                () => context.Service.CreateAsync(CreateRequest(Utc(10), Utc(11))));
        }

        [Fact]
        public async Task CreateAsync_WhenTheBookingStartsExactlyWhenAnotherEnds_IsAllowed()
        {
            var context = new BookingServiceContext(ActiveBooking(Utc(10), Utc(11)));

            var booking = await context.Service.CreateAsync(CreateRequest(Utc(11), Utc(12)));

            Assert.Equal("Active", booking.Status);
            Assert.Equal(2, context.Bookings.Count);
        }

        [Fact]
        public async Task CreateAsync_WhenTheBookingEndsExactlyWhenAnotherStarts_IsAllowed()
        {
            var context = new BookingServiceContext(ActiveBooking(Utc(11), Utc(12)));

            var booking = await context.Service.CreateAsync(CreateRequest(Utc(10), Utc(11)));

            Assert.Equal("Active", booking.Status);
            Assert.Equal(2, context.Bookings.Count);
        }

        [Fact]
        public async Task CreateAsync_WhenTheOverlappingBookingIsCancelled_IsAllowed()
        {
            var context = new BookingServiceContext(CancelledBooking(Utc(10), Utc(11)));

            var booking = await context.Service.CreateAsync(CreateRequest(Utc(10), Utc(11)));

            Assert.Equal("Active", booking.Status);
        }

        [Fact]
        public async Task CreateAsync_WhenAnotherResourceIsBookedAtTheSameTime_IsAllowed()
        {
            var context = new BookingServiceContext(ActiveBooking(Utc(10), Utc(11), resourceId: "room-2"));

            var booking = await context.Service.CreateAsync(CreateRequest(Utc(10), Utc(11)));

            Assert.Equal(ResourceId, booking.ResourceId);
        }

        [Fact]
        public async Task CreateAsync_TrimsTheProvidedIdentifiers()
        {
            var context = new BookingServiceContext();
            var request = CreateRequest(Utc(10), Utc(11), resourceId: "  room-1  ", userId: "  user-1  ");

            var booking = await context.Service.CreateAsync(request);

            Assert.Equal(ResourceId, booking.ResourceId);
            Assert.Equal(UserId, booking.UserId);
            Assert.Equal(ResourceId, Assert.Single(context.AuditLogs).ResourceId);
        }
    }
}
