using BookingManagement.MVC.Helpers;
using BookingManagement.MVC.Models.AuditLogs;
using BookingManagement.MVC.Services.AuditLogs;
using Microsoft.AspNetCore.Mvc;

namespace BookingManagement.MVC.Controllers
{
    public class AuditLogsController : Controller
    {
        private readonly IAuditLogAppService _auditLogAppService;

        public AuditLogsController(IAuditLogAppService auditLogAppService)
        {
            _auditLogAppService = auditLogAppService;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] AuditLogViewModel auditLog)
        {
            var result = await _auditLogAppService.GetAsync(auditLog.ToInputDto());

            if (!result.IsSuccess || result.Data is null)
            {
                auditLog.ErrorMessage = ApiFailureMessages.Query(result, "The audit log could not be loaded.");

                return View(auditLog);
            }

            auditLog.AuditLogs = result.Data.Items.Select(AuditLogRowViewModel.FromDto).ToList();
            auditLog.ApplyPaging(result.Data);

            return View(auditLog);
        }
    }
}
