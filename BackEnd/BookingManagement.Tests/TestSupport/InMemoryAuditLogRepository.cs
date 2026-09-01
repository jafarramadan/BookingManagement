using BookingManagement.Common.Models;
using BookingManagement.DAL.Interfaces;
using BookingManagement.Domain.Entities;

namespace BookingManagement.Tests.TestSupport
{
    internal class InMemoryAuditLogRepository : IAuditLogRepository
    {
        private readonly List<AuditLog> _auditLogs;

        public InMemoryAuditLogRepository(params AuditLog[] auditLogs)
        {
            _auditLogs = [.. auditLogs];
        }

        public IReadOnlyList<AuditLog> AuditLogs => _auditLogs;

        public Task<PagedResult<AuditLog>> GetAsync(
            Guid? bookingId,
            int page,
            int pageSize)
        {
            var auditLogs = _auditLogs
                .Where(auditLog => !bookingId.HasValue || auditLog.BookingId == bookingId.Value)
                .OrderByDescending(auditLog => auditLog.OccurredAt)
                .ToList();

            var page1 = auditLogs.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Task.FromResult(new PagedResult<AuditLog>(page1, page, pageSize, auditLogs.Count));
        }

        public void Add(AuditLog auditLog)
        {
            _auditLogs.Add(auditLog);
        }
    }
}
