using BookingManagement.Common.DTOs.V1;
using BookingManagement.Common.Models;

namespace BookingManagement.MVC.Services.AuditLogs
{
    public interface IAuditLogAppService
    {
        Task<ApiResult<PagedResult<AuditLogDto>>> GetAsync(AuditLogQueryRequest input);
    }
}
