namespace BookingManagement.Common.DTOs.V1
{
    // What the MVC client needs in order to call GET /resources/{resourceId}/bookings: the resource
    // travels in the route rather than the query string, so this is not the same shape as
    // BookingQueryRequest, which is only the query-string part of the API contract.
    public class GetBookingsInputDto
    {
        public string ResourceId { get; set; } = string.Empty;

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public bool IncludeCancelled { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
