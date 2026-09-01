using BookingManagement.Common.Models;
using BookingManagement.Domain.Entities;

namespace BookingManagement.DAL.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<PagedResult<AuditLog>> GetAsync(Guid? bookingId, int page, int pageSize);

        void Add(AuditLog auditLog);
    }
}
