using BookingManagement.Common.DTOs.V1;

namespace BookingManagement.MVC.Models.Bookings
{
    public class DashboardViewModel
    {
        public CreateBookingViewModel Create { get; set; } = new();

        public BookingSearchViewModel Search { get; set; } = new();

        // Loaded from GET /api/v1/resources. The API owns the list of bookable resources; the UI
        // only renders it, and the API rejects a booking for anything not on it.
        public IReadOnlyList<ResourceDto> Resources { get; set; } = [];

        public string? ResourcesErrorMessage { get; set; }

        public bool HasResources => Resources.Count > 0;

        // The stored id of a booked resource may no longer be in the catalog (retired resource),
        // so the dropdown keeps showing the current selection rather than silently dropping it.
        public string? DisplayNameFor(string? resourceId)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
            {
                return null;
            }

            var resource = Resources.FirstOrDefault(
                candidate => string.Equals(candidate.Id, resourceId, StringComparison.OrdinalIgnoreCase));

            return resource?.Name ?? resourceId;
        }

        public IDictionary<string, string> ToRouteData(int? page = null) => Search.ToRouteData(page);
    }
}
