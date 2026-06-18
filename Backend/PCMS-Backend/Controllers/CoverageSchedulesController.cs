using Microsoft.AspNetCore.Mvc;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Shared;


namespace PCMS_Backend.Controllers;

[Route("api/coverage-schedules")]
[ApiController]
public class CoverageSchedulesController : ControllerBase
{
    private readonly ICoverageScheduleService _coverageScheduleService;

    public CoverageSchedulesController(
        ICoverageScheduleService coverageScheduleService)
    {
        _coverageScheduleService = coverageScheduleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSchedules()
    {
        var result = await _coverageScheduleService
            .GetAllSchedulesAsync();

        return result.ToActionResult();
    }
}