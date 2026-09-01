using BookingManagement.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.DAL.Implementation
{
    internal static class PagedQueryExtensions
    {
        // Every repository pages the same way and only the ordering differs, so the caller orders
        // first: taking IOrderedQueryable makes paging an unordered query a compile error, which is
        // what would otherwise let rows repeat or vanish between pages.
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
            this IOrderedQueryable<T> query,
            int page,
            int pageSize)
        {
            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<T>(items, page, pageSize, totalCount);
        }
    }
}
