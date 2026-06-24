using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Services;
using PCMS_Backend.Shared;
using System.Security.Claims;


namespace PCMS_Backend.Controllers;
[Authorize(Roles = "Supervisor")]
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


    [HttpPost("generate")]
    public async Task<IActionResult> Generate()
    {
        if (User.GetCurrentUserId() is not int validUserId) return Unauthorized("Invalid session token.");
        var result = await _coverageScheduleService.GenerateScheduleAsync(validUserId);

        return result.ToActionResult();
    }


    [HttpPost("{id}/publish")]
    public async Task<IActionResult> Publish(int id)
    {
        if (User.GetCurrentUserId() is not int validUserId) return Unauthorized("Invalid session token.");

        var result = await _coverageScheduleService
            .PublishScheduleAsync(id, validUserId);

        return result.ToActionResult();
    }
}