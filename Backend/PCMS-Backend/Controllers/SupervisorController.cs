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

    public SupervisorController(ISupervisorService supervisorService)
    {
        _supervisorService = supervisorService;
    }

    [HttpGet("open-gap-alerts")]
    [Authorize]
    public async Task<IActionResult> GetOpenGapAlerts()
    {
        // Depending on how you implemented RBAC in your JWT, 
        // you might want to add a check here to ensure the user is actually a Supervisor.

        var result = await _supervisorService.GetOpenGapAlertsAsync();
        return result.ToActionResult();
    }
}