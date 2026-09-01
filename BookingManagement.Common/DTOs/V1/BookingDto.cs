namespace BookingManagement.Common.DTOs.V1
{
    public class BookingDto
    {
        public Guid Id { get; set; }

        public string ResourceId { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? CancelledAt { get; set; }
    }
}
