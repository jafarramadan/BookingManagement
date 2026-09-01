using System.Linq.Expressions;
using BookingManagement.Domain.Entities;
using BookingManagement.Common.Enums;
using BookingManagement.Domain.ValueObjects;

namespace BookingManagement.Domain.Specifications
{
    public static class BookingSpecifications
    {
        // Single source of truth for the overlap rule: EF Core translates it to SQL when a
        // booking is created, and the unit tests run the same expression in memory.
        public static Expression<Func<Booking, bool>> ActiveOverlapping(string resourceId, TimeRange range)
        {
            return booking => booking.ResourceId == resourceId
                              && booking.Status == BookingStatus.Active
                              && booking.StartDateTime < range.End
                              && booking.EndDateTime > range.Start;
        }
    }
}
