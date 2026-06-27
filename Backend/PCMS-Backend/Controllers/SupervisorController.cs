using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Controllers;

[ApiController]
[Authorize (Roles = "Supervisor")]
[Route("api/supervisor")] 
public class SupervisorController : ControllerBase
{
    private readonly ISupervisorService _supervisorService;

    private readonly ICoverageScheduleService _coverageScheduleService;

    public SupervisorController(ISupervisorService supervisorService, ICoverageScheduleService coverageScheduleService)
    {
        _supervisorService = supervisorService;
        _coverageScheduleService = coverageScheduleService;
    }


    [HttpGet("dashboard/top-per-specialty")]
    public async Task<IActionResult> GetTopPerSpecialty()
    {
        var result = await _coverageScheduleService.GetTopPerSpecialty();

        return result.ToActionResult();
    }


    [HttpGet("dashboard/details")]
    public async Task<IActionResult> GetDashboardDetails()
    {
        var result = await _supervisorService.DashboardDetails();

        return result.ToActionResult();
    }

}