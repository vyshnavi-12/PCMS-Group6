using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Shared;


namespace PCMS_Backend.Controllers;
[Authorize(Roles ="Supervisor")]
[Route("api/[controller]")]
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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetScheduleById([FromRoute] int id)
    {
        var result = await _coverageScheduleService
            .GetScheduleByIdAsync(id);

        return result.ToActionResult();
    }
}