using BookingManagement.Common.DTOs.V1;
using BookingManagement.Common.Exceptions;
using BookingManagement.Tests.TestSupport;
using static BookingManagement.Tests.TestSupport.BookingTestData;

namespace BookingManagement.Tests.BL
{
    public class BookingServiceRetrievalTests
    {
        [Fact]
        public async Task GetForResourceAsync_ReturnsMappedBookingsWithPagingInformation()
        {
            var context = new BookingServiceContext(
                ActiveBooking(Utc(9), Utc(10)),
                ActiveBooking(Utc(11), Utc(12)));

            var result = await context.Service.GetForResourceAsync(ResourceId, new BookingQueryRequest());

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(1, result.Page);
            Assert.Equal(50, result.PageSize);
            Assert.Equal(new[] { Utc(9), Utc(11) }, result.Items.Select(booking => booking.StartDateTime));
            Assert.All(result.Items, booking => Assert.Equal("Active", booking.Status));
        }

        [Fact]
        public async Task GetForResourceAsync_PassesTheFiltersToTheRepository()
        {
            var context = new BookingServiceContext();
            var query = new BookingQueryRequest
            {
                From = Utc(9),
                To = Utc(17),
                IncludeCancelled = true,
                Page = 2,
                PageSize = 10
            };

            await context.Service.GetForResourceAsync("  room-1  ", query);

            var arguments = Assert.IsType<BookingQueryArguments>(context.LastResourceQuery);
            Assert.Equal(ResourceId, arguments.ResourceId);
            Assert.Equal(Utc(9), arguments.From);
            Assert.Equal(Utc(17), arguments.To);
            Assert.True(arguments.IncludeCancelled);
            Assert.Equal(2, arguments.Page);
            Assert.Equal(10, arguments.PageSize);
        }

        [Fact]
        public async Task GetForResourceAsync_ExcludesCancelledBookingsByDefault()
        {
            var context = new BookingServiceContext();

            await context.Service.GetForResourceAsync(ResourceId, new BookingQueryRequest());

            Assert.False(context.LastResourceQuery!.IncludeCancelled);
        }

        [Theory]
        [InlineData(0, 50)]
        [InlineData(-1, 50)]
        [InlineData(1, 0)]
        [InlineData(1, 201)]
        public async Task GetForResourceAsync_WithInvalidPaging_IsRejected(int page, int pageSize)
        {
            var context = new BookingServiceContext();
            var query = new BookingQueryRequest { Page = page, PageSize = pageSize };

            await Assert.ThrowsAsync<BusinessValidationException>(
                () => context.Service.GetForResourceAsync(ResourceId, query));
        }

        [Fact]
        public async Task GetForResourceAsync_WhenFromIsNotEarlierThanTo_IsRejected()
        {
            var context = new BookingServiceContext();
            var query = new BookingQueryRequest { From = Utc(17), To = Utc(9) };

            await Assert.ThrowsAsync<BusinessValidationException>(
                () => context.Service.GetForResourceAsync(ResourceId, query));
        }

        [Fact]
        public async Task GetForResourceAsync_WhenTheRangeIsNotUtc_IsRejected()
        {
            var context = new BookingServiceContext();
            var query = new BookingQueryRequest { From = DateTime.SpecifyKind(Utc(9), DateTimeKind.Unspecified) };

            await Assert.ThrowsAsync<BusinessValidationException>(
                () => context.Service.GetForResourceAsync(ResourceId, query));
        }

        [Fact]
        public async Task GetForResourceAsync_WithoutResourceId_IsRejected()
        {
            var context = new BookingServiceContext();

            await Assert.ThrowsAsync<BusinessValidationException>(
                () => context.Service.GetForResourceAsync("  ", new BookingQueryRequest()));
        }
    }
}
