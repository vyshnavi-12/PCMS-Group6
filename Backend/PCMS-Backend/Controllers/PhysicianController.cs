using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Services;
using PCMS_Backend.Shared;
using System.Security.Claims;

namespace PCMS_Backend.Controllers;

[ApiController]
[Route("api/physician")]
public class PhysicianController : ControllerBase
{
    private readonly IPhysicianService _physicianService;

    public PhysicianController(IPhysicianService physicianService)
    {
        _physicianService = physicianService;
    }

    [Authorize(Roles = "Physician")]
    [HttpGet("assignments")]
    public async Task<IActionResult> GetAllAssignments()

    {

        if (User.GetCurrentUserId() is not int validUserId) return Unauthorized("Invalid session token.");

      
       
            var result = await _physicianService.GetAllAssignmentsAsync(validUserId);
            return result.ToActionResult();
        
         
    }

    [Authorize(Roles = "Physician")]
    [HttpGet("unavailable-requests-count")]
    public async Task<IActionResult> GetUnavailableRequestCount()
    {
        if (User.GetCurrentUserId() is not int validUserId) return Unauthorized("Missing token");
        var result = await _physicianService.GetUnavailableRequestCountAsync(validUserId);
        return result.ToActionResult();
    }

    [Authorize(Roles = "Supervisor")]
    [HttpGet("{physicianId}/details")]
    public async Task<IActionResult> GetPhysicianDetails(int physicianId)
    {
        var result = await _physicianService.GetPhysicianDetailsByIdAsync(physicianId);
        return result.ToActionResult();
    }
}

