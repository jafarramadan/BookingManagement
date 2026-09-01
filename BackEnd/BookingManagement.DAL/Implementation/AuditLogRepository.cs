using BookingManagement.Common.Models;
using BookingManagement.DAL.Interfaces;
using BookingManagement.Domain.Entities;

namespace BookingManagement.DAL.Implementation
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly BookingDbContext _dbContext;

        public AuditLogRepository(BookingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<AuditLog>> GetAsync(
            Guid? bookingId,
            int page,
            int pageSize)
        {
            var query = _dbContext.AuditLogs.AsQueryable();

            if (bookingId.HasValue)
            {
                query = query.Where(auditLog => auditLog.BookingId == bookingId.Value);
            }

            return await query
                .OrderByDescending(auditLog => auditLog.OccurredAt)
                .ThenByDescending(auditLog => auditLog.Id)
                .ToPagedResultAsync(page, pageSize);
        }

        public void Add(AuditLog auditLog)
        {
            _dbContext.AuditLogs.Add(auditLog);
        }
    }
}
