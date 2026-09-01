using BookingManagement.Common.Enums;

namespace BookingManagement.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }

        public Guid BookingId { get; set; }

        public AuditEventType EventType { get; set; }

        public DateTime OccurredAt { get; set; }

        public string ResourceId { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        // The record copies the resource and user of the booking so the trail stays readable
        // on its own, the way a stored business event would.
        public static AuditLog For(Booking booking, AuditEventType eventType, DateTime occurredAt)
        {
            return new AuditLog
            {
                Id = Guid.NewGuid(),
                BookingId = booking.Id,
                EventType = eventType,
                OccurredAt = occurredAt,
                ResourceId = booking.ResourceId,
                UserId = booking.UserId
            };
        }
    }
}
