using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCMS_Backend.Services;
using PCMS_Backend.Shared;
using PCMS_Backend.DTOs;
using System.Threading.Tasks;

namespace PCMS_Backend.Controllers
{
    [Authorize(Roles = "Supervisor")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _service;

        public AuditLogsController(IAuditLogService service)
        {
            _service = service;
        }

        // GET /api/audit-logs
        [HttpGet]
        public async Task<IActionResult> GetAuditLogs()
        {
            var result = await _service.GetAuditLogsAsync();
            return result.ToActionResult();
        }

        // POST /api/audit-logs
        [HttpPost]
        public async Task<IActionResult> CreateAuditLog([FromBody] AuditLogDto request)
        {
            var result = await _service.LogActionAsync(
                request.ActionType,
                request.EntityName,
                request.EntityRecordId,
                request.PerformedByUserId
            );

            return result.ToActionResult();
        }
    }
}
