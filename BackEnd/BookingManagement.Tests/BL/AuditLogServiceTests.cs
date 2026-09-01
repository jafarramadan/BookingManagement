using BookingManagement.BL.Implementation;
using BookingManagement.Common.DTOs.V1;
using BookingManagement.Common.Exceptions;
using BookingManagement.Domain.Entities;
using BookingManagement.Common.Enums;
using BookingManagement.Tests.TestSupport;
using static BookingManagement.Tests.TestSupport.BookingTestData;

namespace BookingManagement.Tests.BL
{
    public class AuditLogServiceTests
    {
        [Fact]
        public async Task GetAsync_ReturnsTheNewestEventFirst()
        {
            var booking = ActiveBooking(Utc(10), Utc(11));
            var service = CreateService(
                AuditLog.For(booking, AuditEventType.BookingCreated, Utc(9)),
                AuditLog.For(booking, AuditEventType.BookingCancelled, Utc(12)));

            var result = await service.GetAsync(new AuditLogQueryRequest());

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(new[] { "BookingCancelled", "BookingCreated" }, result.Items.Select(auditLog => auditLog.EventType));
        }

        [Fact]
        public async Task GetAsync_MapsTheEventToItsName()
        {
            var booking = ActiveBooking(Utc(10), Utc(11));
            var service = CreateService(AuditLog.For(booking, AuditEventType.BookingCreated, Utc(9)));

            var auditLog = Assert.Single((await service.GetAsync(new AuditLogQueryRequest())).Items);

            Assert.Equal("BookingCreated", auditLog.EventType);
            Assert.Equal(booking.Id, auditLog.BookingId);
            Assert.Equal(Utc(9), auditLog.OccurredAt);
            Assert.Equal(ResourceId, auditLog.ResourceId);
            Assert.Equal(UserId, auditLog.UserId);
        }

        [Fact]
        public async Task GetAsync_CanBeFilteredByBooking()
        {
            var first = ActiveBooking(Utc(10), Utc(11));
            var second = ActiveBooking(Utc(12), Utc(13));
            var service = CreateService(
                AuditLog.For(first, AuditEventType.BookingCreated, Utc(9)),
                AuditLog.For(second, AuditEventType.BookingCreated, Utc(9, 30)));

            var result = await service.GetAsync(new AuditLogQueryRequest { BookingId = second.Id });

            Assert.Equal(1, result.TotalCount);
            Assert.Equal(second.Id, Assert.Single(result.Items).BookingId);
        }

        [Fact]
        public async Task GetAsync_AppliesPaging()
        {
            var booking = ActiveBooking(Utc(10), Utc(11));
            var service = CreateService(
                AuditLog.For(booking, AuditEventType.BookingCreated, Utc(9)),
                AuditLog.For(booking, AuditEventType.BookingCancelled, Utc(10)),
                AuditLog.For(booking, AuditEventType.BookingCreated, Utc(11)));

            var result = await service.GetAsync(new AuditLogQueryRequest { Page = 2, PageSize = 2 });

            Assert.Equal(3, result.TotalCount);
            Assert.Equal(2, result.TotalPages);
            Assert.Single(result.Items);
        }

        [Theory]
        [InlineData(0, 50)]
        [InlineData(1, 0)]
        [InlineData(1, 201)]
        public async Task GetAsync_WithInvalidPaging_IsRejected(int page, int pageSize)
        {
            var service = CreateService();
            var query = new AuditLogQueryRequest { Page = page, PageSize = pageSize };

            await Assert.ThrowsAsync<BusinessValidationException>(() => service.GetAsync(query));
        }

        private static AuditLogService CreateService(params AuditLog[] auditLogs)
        {
            return new AuditLogService(new InMemoryAuditLogRepository(auditLogs));
        }
    }
}
