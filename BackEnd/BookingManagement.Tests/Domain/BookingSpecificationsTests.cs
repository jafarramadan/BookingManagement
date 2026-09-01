using BookingManagement.Domain.Entities;
using BookingManagement.Domain.Specifications;
using BookingManagement.Domain.ValueObjects;
using static BookingManagement.Tests.TestSupport.BookingTestData;

namespace BookingManagement.Tests.Domain
{
    public class BookingSpecificationsTests
    {
        [Fact]
        public void ActiveOverlapping_ExcludesBookingThatEndsWhenTheRequestedSlotStarts()
        {
            var existing = ActiveBooking(Utc(9), Utc(10));

            Assert.False(Matches(existing, Utc(10), Utc(11)));
        }

        [Fact]
        public void ActiveOverlapping_ExcludesBookingThatStartsWhenTheRequestedSlotEnds()
        {
            var existing = ActiveBooking(Utc(11), Utc(12));

            Assert.False(Matches(existing, Utc(10), Utc(11)));
        }

        [Fact]
        public void ActiveOverlapping_IncludesPartiallyOverlappingBooking()
        {
            var existing = ActiveBooking(Utc(10, 30), Utc(11, 30));

            Assert.True(Matches(existing, Utc(10), Utc(11)));
        }

        [Fact]
        public void ActiveOverlapping_IncludesBookingContainedInTheRequestedSlot()
        {
            var existing = ActiveBooking(Utc(10, 15), Utc(10, 45));

            Assert.True(Matches(existing, Utc(10), Utc(11)));
        }

        [Fact]
        public void ActiveOverlapping_IncludesBookingThatContainsTheRequestedSlot()
        {
            var existing = ActiveBooking(Utc(9), Utc(17));

            Assert.True(Matches(existing, Utc(10), Utc(11)));
        }

        [Fact]
        public void ActiveOverlapping_ExcludesCancelledBooking()
        {
            var existing = CancelledBooking(Utc(10), Utc(11));

            Assert.False(Matches(existing, Utc(10), Utc(11)));
        }

        [Fact]
        public void ActiveOverlapping_ExcludesBookingOfAnotherResource()
        {
            var existing = ActiveBooking(Utc(10), Utc(11), resourceId: "room-2");

            Assert.False(Matches(existing, Utc(10), Utc(11)));
        }

        private static bool Matches(Booking booking, DateTime start, DateTime end)
        {
            var specification = BookingSpecifications
                .ActiveOverlapping(ResourceId, new TimeRange(start, end))
                .Compile();

            return specification(booking);
        }
    }
}
