using BookingManagement.Common.Enums;
using BookingManagement.Domain.ValueObjects;

namespace BookingManagement.Domain.Entities
{
    public class Booking
    {
        public Guid Id { get; set; }

        public string ResourceId { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public BookingStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? CancelledAt { get; set; }

        public bool IsActive => Status == BookingStatus.Active;

        public static Booking Create(string resourceId, string userId, TimeRange slot, DateTime createdAt)
        {
            return new Booking
            {
                Id = Guid.NewGuid(),
                ResourceId = resourceId,
                UserId = userId,
                StartDateTime = slot.Start,
                EndDateTime = slot.End,
                Status = BookingStatus.Active,
                CreatedAt = createdAt
            };
        }

        public void Cancel(DateTime cancelledAt)
        {
            Status = BookingStatus.Cancelled;
            CancelledAt = cancelledAt;
        }
    }
}
