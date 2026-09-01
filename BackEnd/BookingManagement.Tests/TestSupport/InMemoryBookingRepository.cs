using BookingManagement.Common.Models;
using BookingManagement.DAL.Interfaces;
using BookingManagement.Domain.Entities;
using BookingManagement.Domain.Specifications;
using BookingManagement.Domain.ValueObjects;

namespace BookingManagement.Tests.TestSupport
{
    // Overlap queries are answered by compiling the production specification, so the business
    // rule under test is the same expression the repository sends to PostgreSQL.
    internal class InMemoryBookingRepository : IBookingRepository
    {
        private readonly List<Booking> _bookings;

        public InMemoryBookingRepository(params Booking[] bookings)
        {
            _bookings = [.. bookings];
        }

        public IReadOnlyList<Booking> Bookings => _bookings;

        public BookingQueryArguments? LastResourceQuery { get; private set; }

        public Task<Booking?> GetByIdAsync(Guid bookingId)
        {
            return Task.FromResult(_bookings.FirstOrDefault(booking => booking.Id == bookingId));
        }

        public Task<bool> HasActiveOverlapAsync(string resourceId, TimeRange slot)
        {
            var specification = BookingSpecifications.ActiveOverlapping(resourceId, slot).Compile();

            return Task.FromResult(_bookings.Any(specification));
        }

        public Task<PagedResult<Booking>> GetForResourceAsync(
            string resourceId,
            DateTime? from,
            DateTime? to,
            bool includeCancelled,
            int page,
            int pageSize)
        {
            LastResourceQuery = new BookingQueryArguments(resourceId, from, to, includeCancelled, page, pageSize);

            var bookings = _bookings
                .Where(booking => booking.ResourceId == resourceId)
                .OrderBy(booking => booking.StartDateTime)
                .ToList();

            return Task.FromResult(new PagedResult<Booking>(bookings, page, pageSize, bookings.Count));
        }

        public void Add(Booking booking)
        {
            _bookings.Add(booking);
        }
    }

    internal record BookingQueryArguments(
        string ResourceId,
        DateTime? From,
        DateTime? To,
        bool IncludeCancelled,
        int Page,
        int PageSize);
}
