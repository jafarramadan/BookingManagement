using BookingManagement.MVC.Services.Common;
using BookingManagement.Common.DTOs.V1;
using BookingManagement.Common.Models;
using Microsoft.Extensions.Logging;

namespace BookingManagement.MVC.Services.AuditLogs
{
    public class AuditLogAppService : ApiServiceBase, IAuditLogAppService
    {
        public AuditLogAppService(HttpClient httpClient, ILogger<AuditLogAppService> logger)
            : base(httpClient, logger)
        {
        }

        public Task<ApiResult<PagedResult<AuditLogDto>>> GetAsync(AuditLogQueryRequest input)
        {
            var query = new List<string>
            {
                $"page={input.Page}",
                $"pageSize={input.PageSize}"
            };

            if (input.BookingId.HasValue)
            {
                query.Add($"bookingId={input.BookingId.Value}");
            }

            var url = $"api/v1/audit-logs?{string.Join('&', query)}";

            return SendAsync<PagedResult<AuditLogDto>>(() => HttpClient.GetAsync(url));
        }
    }
}
