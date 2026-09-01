using BookingManagement.Common.DTOs.V1;
using BookingManagement.Common.Models;

namespace BookingManagement.BL.Interfaces
{
    public interface IBookingService
    {
        Task<BookingDto> CreateAsync(CreateBookingRequest request);

        Task<BookingDto> GetByIdAsync(Guid bookingId);

        Task<BookingDto> CancelAsync(Guid bookingId);

        Task<PagedResult<BookingDto>> GetForResourceAsync(string resourceId, BookingQueryRequest query);
    }
}
