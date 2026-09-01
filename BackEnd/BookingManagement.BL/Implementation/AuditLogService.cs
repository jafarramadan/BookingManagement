using BookingManagement.BL.Interfaces;
using BookingManagement.BL.Validation;
using BookingManagement.Common.DTOs.V1;
using BookingManagement.Common.Models;
using BookingManagement.DAL.Interfaces;
using BookingManagement.Domain.Entities;

namespace BookingManagement.BL.Implementation
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task<PagedResult<AuditLogDto>> GetAsync(AuditLogQueryRequest query)
        {
            PagingValidator.Validate(query.Page, query.PageSize);

            var auditLogs = await _auditLogRepository.GetAsync(query.BookingId, query.Page, query.PageSize);

            return new PagedResult<AuditLogDto>(
                auditLogs.Items.Select(ToDto).ToList(),
                auditLogs.Page,
                auditLogs.PageSize,
                auditLogs.TotalCount);
        }

        private static AuditLogDto ToDto(AuditLog auditLog)
        {
            return new AuditLogDto
            {
                Id = auditLog.Id,
                BookingId = auditLog.BookingId,
                EventType = auditLog.EventType.ToString(),
                OccurredAt = auditLog.OccurredAt,
                ResourceId = auditLog.ResourceId,
                UserId = auditLog.UserId
            };
        }
    }
}
