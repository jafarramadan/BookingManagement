using BookingManagement.Common.DTOs.V1;
using BookingManagement.Common.Models;

namespace BookingManagement.BL.Interfaces
{
    public interface IAuditLogService
    {
        Task<PagedResult<AuditLogDto>> GetAsync(AuditLogQueryRequest query);
    }
}
