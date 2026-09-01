namespace BookingManagement.Common.DTOs.V1
{
    public class AuditLogQueryRequest
    {
        public Guid? BookingId { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 50;
    }
}
