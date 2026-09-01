using BookingManagement.Common.Exceptions;
using BookingManagement.Common.Enums;
using BookingManagement.Tests.TestSupport;
using static BookingManagement.Tests.TestSupport.BookingTestData;

namespace BookingManagement.Tests.BL
{
    public class BookingServiceCancelTests
    {
        [Fact]
        public async Task CancelAsync_WithAnActiveBooking_MarksItCancelled()
        {
            var existing = ActiveBooking(Utc(10), Utc(11));
            var context = new BookingServiceContext(existing);

            var booking = await context.Service.CancelAsync(existing.Id);

            Assert.Equal("Cancelled", booking.Status);
            Assert.NotNull(booking.CancelledAt);
            Assert.Equal(DateTimeKind.Utc, booking.CancelledAt!.Value.Kind);
            Assert.Single(context.Bookings);
            Assert.Equal(1, context.SaveChangesCount);
        }

        [Fact]
        public async Task CancelAsync_WritesABookingCancelledAuditLog()
        {
            var existing = ActiveBooking(Utc(10), Utc(11));
            var context = new BookingServiceContext(existing);

            var booking = await context.Service.CancelAsync(existing.Id);

            var auditLog = Assert.Single(context.AuditLogs);
            Assert.Equal(AuditEventType.BookingCancelled, auditLog.EventType);
            Assert.Equal(existing.Id, auditLog.BookingId);
            Assert.Equal(booking.CancelledAt!.Value, auditLog.OccurredAt);
            Assert.Equal(ResourceId, auditLog.ResourceId);
            Assert.Equal(UserId, auditLog.UserId);
        }

        [Fact]
        public async Task CancelAsync_WhenTheBookingDoesNotExist_IsRejected()
        {
            var context = new BookingServiceContext();

            await Assert.ThrowsAsync<NotFoundException>(() => context.Service.CancelAsync(Guid.NewGuid()));
            Assert.Empty(context.AuditLogs);
            Assert.Equal(0, context.SaveChangesCount);
        }

        [Fact]
        public async Task CancelAsync_WhenTheBookingIsAlreadyCancelled_IsRejected()
        {
            var existing = CancelledBooking(Utc(10), Utc(11));
            var context = new BookingServiceContext(existing);

            await Assert.ThrowsAsync<ConflictException>(() => context.Service.CancelAsync(existing.Id));
            Assert.Empty(context.AuditLogs);
            Assert.Equal(0, context.SaveChangesCount);
        }

        [Fact]
        public async Task CancelAsync_ReleasesTheSlotForANewBooking()
        {
            var existing = ActiveBooking(Utc(10), Utc(11));
            var context = new BookingServiceContext(existing);

            await context.Service.CancelAsync(existing.Id);
            var booking = await context.Service.CreateAsync(CreateRequest(Utc(10), Utc(11)));

            Assert.Equal("Active", booking.Status);
            Assert.Equal(2, context.Bookings.Count);
        }

        [Fact]
        public async Task CreateThenCancel_LeavesBothEventsInTheAuditTrail()
        {
            var context = new BookingServiceContext();

            var created = await context.Service.CreateAsync(CreateRequest(Utc(10), Utc(11)));
            await context.Service.CancelAsync(created.Id);

            var expectedEvents = new[] { AuditEventType.BookingCreated, AuditEventType.BookingCancelled };

            Assert.Equal(2, context.AuditLogs.Count);
            Assert.Equal(expectedEvents, context.AuditLogs.Select(auditLog => auditLog.EventType));
            Assert.All(context.AuditLogs, auditLog => Assert.Equal(created.Id, auditLog.BookingId));
            Assert.Equal(2, context.SaveChangesCount);
        }
    }
}
