namespace BookingManagement.Common.DTOs.V1
{
    public class BookingQueryRequest
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public bool IncludeCancelled { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 50;
    }
}
