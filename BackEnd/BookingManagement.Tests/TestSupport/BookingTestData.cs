using BookingManagement.Common.DTOs.V1;
using BookingManagement.Domain.Entities;
using BookingManagement.Domain.ValueObjects;

namespace BookingManagement.Tests.TestSupport
{
    internal static class BookingTestData
    {
        public const string ResourceId = "room-1";
        public const string UserId = "user-1";

        private static readonly DateTime BookingDay = new(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime Utc(int hour, int minute = 0) => BookingDay.AddHours(hour).AddMinutes(minute);

        public static Booking ActiveBooking(DateTime start, DateTime end, string resourceId = ResourceId)
        {
            return Booking.Create(resourceId, UserId, new TimeRange(start, end), Utc(0));
        }

        public static Booking CancelledBooking(DateTime start, DateTime end, string resourceId = ResourceId)
        {
            var booking = ActiveBooking(start, end, resourceId);
            booking.Cancel(Utc(1));

            return booking;
        }

        public static CreateBookingRequest CreateRequest(
            DateTime start,
            DateTime end,
            string? resourceId = ResourceId,
            string? userId = UserId)
        {
            return new CreateBookingRequest
            {
                ResourceId = resourceId,
                UserId = userId,
                StartDateTime = start,
                EndDateTime = end
            };
        }
    }
}
