namespace BookingManagement.Common.DTOs.V1
{
    public class CreateBookingRequest
    {
        public string? ResourceId { get; set; }

        public string? UserId { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }
    }
}
