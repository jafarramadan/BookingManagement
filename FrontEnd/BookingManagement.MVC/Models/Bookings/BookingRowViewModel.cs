using BookingManagement.Common.DTOs.V1;

namespace BookingManagement.MVC.Models.Bookings
{
    public class BookingRowViewModel
    {
        public Guid Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string Status { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public static BookingRowViewModel FromDto(BookingDto booking)
        {
            return new BookingRowViewModel
            {
                Id = booking.Id,
                UserId = booking.UserId,
                StartDateTime = booking.StartDateTime,
                EndDateTime = booking.EndDateTime,
                Status = booking.Status,
                IsActive = string.Equals(booking.Status, "Active", StringComparison.OrdinalIgnoreCase)
            };
        }
    }
}
