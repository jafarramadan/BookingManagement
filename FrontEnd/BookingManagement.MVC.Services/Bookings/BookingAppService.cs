using System.Net.Http.Json;
using BookingManagement.MVC.Services.Common;
using BookingManagement.Common.DTOs.V1;
using BookingManagement.Common.Models;
using Microsoft.Extensions.Logging;

namespace BookingManagement.MVC.Services.Bookings
{
    public class BookingAppService : ApiServiceBase, IBookingAppService
    {
        public BookingAppService(HttpClient httpClient, ILogger<BookingAppService> logger)
            : base(httpClient, logger)
        {
        }

        public Task<ApiResult<BookingDto>> CreateAsync(CreateBookingRequest input)
        {
            return SendAsync<BookingDto>(
                () => HttpClient.PostAsJsonAsync("api/v1/bookings", input, JsonOptions));
        }

        public Task<ApiResult<BookingDto>> CancelAsync(Guid bookingId)
        {
            return SendAsync<BookingDto>(
                () => HttpClient.PostAsync($"api/v1/bookings/{bookingId}/cancel", content: null));
        }

        public Task<ApiResult<PagedResult<BookingDto>>> GetForResourceAsync(GetBookingsInputDto input)
        {
            var query = new List<string>
            {
                $"includeCancelled={input.IncludeCancelled.ToString().ToLowerInvariant()}",
                $"page={input.Page}",
                $"pageSize={input.PageSize}"
            };

            if (input.From.HasValue)
            {
                query.Add($"from={FormatUtc(input.From.Value)}");
            }

            if (input.To.HasValue)
            {
                query.Add($"to={FormatUtc(input.To.Value)}");
            }

            var url = $"api/v1/resources/{Uri.EscapeDataString(input.ResourceId)}/bookings?{string.Join('&', query)}";

            return SendAsync<PagedResult<BookingDto>>(() => HttpClient.GetAsync(url));
        }
    }
}
