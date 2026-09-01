using BookingManagement.Common.Models;
using BookingManagement.Domain.Entities;
using BookingManagement.Domain.ValueObjects;

namespace BookingManagement.DAL.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(Guid bookingId);

        Task<bool> HasActiveOverlapAsync(string resourceId, TimeRange slot);

        Task<PagedResult<Booking>> GetForResourceAsync(
            string resourceId,
            DateTime? from,
            DateTime? to,
            bool includeCancelled,
            int page,
            int pageSize);

        void Add(Booking booking);
    }
}
