using BookingManagement.Common.Models;

namespace BookingManagement.MVC.Models
{
    // Both list pages carry the same paging state and render the same pager, so the state,
    // the "copy the paged result onto the view model" step and _Pager.cshtml are shared.
    public abstract class PagedViewModel
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

        public string? ErrorMessage { get; set; }

        public bool HasPreviousPage => Page > 1;

        public bool HasNextPage => Page < TotalPages;

        // Singular noun for the pager summary, which reads "12 booking(s)".
        public abstract string ItemNoun { get; }

        // The query-string keys differ per page, so each view model builds its own.
        public abstract IDictionary<string, string> ToRouteData(int? page = null);

        public void ApplyPaging<T>(PagedResult<T> result)
        {
            Page = result.Page;
            PageSize = result.PageSize;
            TotalCount = result.TotalCount;
            TotalPages = result.TotalPages;
        }
    }
}
