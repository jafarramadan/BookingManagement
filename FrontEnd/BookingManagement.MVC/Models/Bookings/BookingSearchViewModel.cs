using System.ComponentModel.DataAnnotations;
using BookingManagement.Common.DTOs.V1;
using BookingManagement.MVC.Helpers;

namespace BookingManagement.MVC.Models.Bookings
{
    public class BookingSearchViewModel : PagedViewModel
    {
        public const string QueryPrefix = "search";

        public BookingSearchViewModel()
        {
            PageSize = 10;
        }

        public override string ItemNoun => "booking";

        [Display(Name = "Resource")]
        public string? ResourceId { get; set; }

        [Display(Name = "From (UTC)")]
        public DateTime? From { get; set; }

        [Display(Name = "To (UTC)")]
        public DateTime? To { get; set; }

        [Display(Name = "Include cancelled")]
        public bool IncludeCancelled { get; set; }

        public IReadOnlyList<BookingRowViewModel> Bookings { get; set; } = [];

        public bool HasResourceId => !string.IsNullOrWhiteSpace(ResourceId);

        public GetBookingsInputDto ToInputDto()
        {
            return new GetBookingsInputDto
            {
                ResourceId = ResourceId!.Trim(),
                From = From?.AsUtc(),
                To = To?.AsUtc(),
                IncludeCancelled = IncludeCancelled,
                Page = Page,
                PageSize = PageSize
            };
        }

        public RouteValueDictionary ToRouteValues(int? page = null)
        {
            var routeValues = new RouteValueDictionary();

            if (HasResourceId)
            {
                routeValues[Key(nameof(ResourceId))] = ResourceId;
            }

            if (From.HasValue)
            {
                routeValues[Key(nameof(From))] = From.Value.ToString("s");
            }

            if (To.HasValue)
            {
                routeValues[Key(nameof(To))] = To.Value.ToString("s");
            }

            if (IncludeCancelled)
            {
                routeValues[Key(nameof(IncludeCancelled))] = true;
            }

            var requestedPage = page ?? Page;

            if (requestedPage > 1)
            {
                routeValues[Key(nameof(Page))] = requestedPage;
            }

            return routeValues;
        }

        public override IDictionary<string, string> ToRouteData(int? page = null)
        {
            return ToRouteValues(page).ToDictionary(entry => entry.Key, entry => entry.Value?.ToString() ?? string.Empty);
        }

        private static string Key(string propertyName) => $"{QueryPrefix}.{propertyName}";
    }
}
