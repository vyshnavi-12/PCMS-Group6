using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCMS_Backend.Interfaces.Services;

namespace PCMS_Backend.Controllers
{
    [Authorize(Roles = "Supervisor")]
    [Route("api/Supervisor/CoverageGapAlerts")]
    [ApiController]
    public class SupervisorCoverageGapAlertsController : ControllerBase
    {
        private readonly ICoverageGapAlertService _service;

        public SupervisorCoverageGapAlertsController(ICoverageGapAlertService service)
        {
            _service = service;
        }

        [HttpGet("open-count")]
        public async Task<IActionResult> GetOpenAlertsCount()
        {
            var result = await _service.GetOpenAlertsCountAsync();
            return StatusCode(result.StatusCode, result);
        }
    }
}
