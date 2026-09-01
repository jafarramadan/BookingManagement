namespace BookingManagement.Common.Models
{
    // One paged envelope for both sides: the API builds it with the constructor, and the MVC client
    // deserialises the same JSON into it. The parameterless constructor plus settable properties are
    // what let System.Text.Json populate it; TotalPages stays computed so the two sides cannot disagree.
    public class PagedResult<T>
    {
        public PagedResult()
        {
        }

        public PagedResult(List<T> items, int page, int pageSize, int totalCount)
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        public List<T> Items { get; set; } = [];

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
