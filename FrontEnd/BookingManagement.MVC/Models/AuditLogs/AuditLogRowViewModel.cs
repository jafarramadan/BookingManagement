using BookingManagement.Common.DTOs.V1;

namespace BookingManagement.MVC.Models.AuditLogs
{
    public class AuditLogRowViewModel
    {
        public Guid BookingId { get; set; }

        public string EventType { get; set; } = string.Empty;

        public DateTime OccurredAt { get; set; }

        public string ResourceId { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        // Unknown event types fall through unchanged, so a new backend event still renders.
        public string EventDisplayName => EventType switch
        {
            "BookingCreated" => "Booking Created",
            "BookingCancelled" => "Booking Cancelled",
            _ => EventType
        };

        public bool IsCancellation => EventType == "BookingCancelled";

        public static AuditLogRowViewModel FromDto(AuditLogDto auditLog)
        {
            return new AuditLogRowViewModel
            {
                BookingId = auditLog.BookingId,
                EventType = auditLog.EventType,
                OccurredAt = auditLog.OccurredAt,
                ResourceId = auditLog.ResourceId,
                UserId = auditLog.UserId
            };
        }
    }
}
