using Asp.Versioning;
using BookingManagement.BL.Interfaces;
using BookingManagement.Common.DTOs.V1;
using BookingManagement.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookingManagement.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/audit-logs")]
    [Produces("application/json")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<AuditLogDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAuditLogs([FromQuery] AuditLogQueryRequest query)
        {
            var auditLogs = await _auditLogService.GetAsync(query);

            return Ok(auditLogs);
        }
    }
}
