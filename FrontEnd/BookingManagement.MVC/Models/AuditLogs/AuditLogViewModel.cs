using BookingManagement.Common.DTOs.V1;

namespace BookingManagement.MVC.Models.AuditLogs
{
    public class AuditLogViewModel : PagedViewModel
    {
        public AuditLogViewModel()
        {
            PageSize = 20;
        }

        public override string ItemNoun => "event";

        public Guid? BookingId { get; set; }

        public IReadOnlyList<AuditLogRowViewModel> AuditLogs { get; set; } = [];

        public AuditLogQueryRequest ToInputDto()
        {
            return new AuditLogQueryRequest
            {
                BookingId = BookingId,
                Page = Page,
                PageSize = PageSize
            };
        }

        public override IDictionary<string, string> ToRouteData(int? page = null)
        {
            var routeData = new Dictionary<string, string>();

            if (BookingId.HasValue)
            {
                routeData[nameof(BookingId)] = BookingId.Value.ToString();
            }

            var requestedPage = page ?? Page;

            if (requestedPage > 1)
            {
                routeData[nameof(Page)] = requestedPage.ToString();
            }

            return routeData;
        }
    }
}
