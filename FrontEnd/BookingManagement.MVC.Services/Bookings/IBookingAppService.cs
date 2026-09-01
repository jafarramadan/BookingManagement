using BookingManagement.Common.DTOs.V1;
using BookingManagement.Common.Models;

namespace BookingManagement.MVC.Services.Bookings
{
    public interface IBookingAppService
    {
        Task<ApiResult<BookingDto>> CreateAsync(CreateBookingRequest input);

        Task<ApiResult<BookingDto>> CancelAsync(Guid bookingId);

        Task<ApiResult<PagedResult<BookingDto>>> GetForResourceAsync(GetBookingsInputDto input);
    }
}
