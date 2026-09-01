namespace BookingManagement.Common.DTOs.V1
{
    public class AuditLogDto
    {
        public Guid Id { get; set; }

        public Guid BookingId { get; set; }

        public string EventType { get; set; } = string.Empty;

        public DateTime OccurredAt { get; set; }

        public string ResourceId { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
    }
}
