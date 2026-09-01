using BookingManagement.Common.Models;
using BookingManagement.DAL.Interfaces;
using BookingManagement.Domain.Entities;
using BookingManagement.Common.Enums;
using BookingManagement.Domain.Specifications;
using BookingManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.DAL.Implementation
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDbContext _dbContext;

        public BookingRepository(BookingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Booking?> GetByIdAsync(Guid bookingId)
        {
            return await _dbContext.Bookings.FirstOrDefaultAsync(booking => booking.Id == bookingId);
        }

        public async Task<bool> HasActiveOverlapAsync(string resourceId, TimeRange slot)
        {
            return await _dbContext.Bookings.AnyAsync(BookingSpecifications.ActiveOverlapping(resourceId, slot));
        }

        public async Task<PagedResult<Booking>> GetForResourceAsync(
            string resourceId,
            DateTime? from,
            DateTime? to,
            bool includeCancelled,
            int page,
            int pageSize)
        {
            var query = _dbContext.Bookings.Where(booking => booking.ResourceId == resourceId);

            if (!includeCancelled)
            {
                query = query.Where(booking => booking.Status == BookingStatus.Active);
            }

            // The date filter uses the same half-open overlap rule as booking creation:
            // a booking is returned when it intersects [from, to).
            if (from.HasValue)
            {
                query = query.Where(booking => booking.EndDateTime > from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(booking => booking.StartDateTime < to.Value);
            }

            return await query
                .OrderBy(booking => booking.StartDateTime)
                .ThenBy(booking => booking.Id)
                .ToPagedResultAsync(page, pageSize);
        }

        public void Add(Booking booking)
        {
            _dbContext.Bookings.Add(booking);
        }
    }
}
