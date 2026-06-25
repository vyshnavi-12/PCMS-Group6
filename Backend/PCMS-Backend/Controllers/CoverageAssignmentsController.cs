using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Services;
using System.Security.Claims;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Controllers;


[ApiController]

[Route("api/[controller]")]
public class CoverageAssignmentsController : ControllerBase
{
    private readonly ICoverageAssignmentsService _coverageService;
    private readonly IPhysicianService _physicianService;

    public CoverageAssignmentsController(ICoverageAssignmentsService coverageService, IPhysicianService physicianService)
    {
        _coverageService = coverageService;
        _physicianService = physicianService;
    }

    [HttpPatch("{assignmentId}/unavailable")]
    [Authorize(Roles = "Physician")]
    public async Task<IActionResult> MarkUnavailable(int assignmentId, [FromBody] UnavailableRequestDto req)
    {
        if(User.GetCurrentUserId() is not int validUserId) return Unauthorized("Invalid session token."); 
        var physicianId = await _physicianService.GetPhysicianIdByUserIdAsync();
        if (physicianId is not int validPhysicianId) return Unauthorized("Not authorized");
        var result = await _coverageService.MarkAssignmentUnavailableAsync(assignmentId, req.Reason, validPhysicianId);
        return result.ToActionResult();
    }

    [HttpGet("alerts")]
    [Authorize(Roles = "Supervisor")]
    public async Task<IActionResult> GetOpenAlerts()
    {
        var result = await _coverageService.GetAlertsAsync();
        return result.ToActionResult();
    }
    [HttpGet("alerts/{alertId}/details")]
    [Authorize(Roles = "Supervisor")]
public async Task<IActionResult> GetOpenAlertDetails(int alertId)
    {
        var result = await _coverageService.GetAlertDetailsAsync(alertId);
        return result.ToActionResult();
    }
    [HttpPatch("alerts/{alertId}/update-physician/{physicianId}")]
    [Authorize(Roles = "Supervisor")]
    public async Task<IActionResult> UpdatePhysician(int alertId, int physicianId)
    {
        var result = await _coverageService.UpdateAlertPhysicianAsync(alertId, physicianId); 
        return result.ToActionResult();
    }

    [HttpPatch("alerts/{alertId}/decline-request")]
    [Authorize(Roles = "Supervisor")]
    public async Task<IActionResult> DeclineUnavailableRequest(int alertId)
    {
        var result = await _coverageService.DeclineUnavailableRequestAsync(alertId);
        return result.ToActionResult();
    }
}