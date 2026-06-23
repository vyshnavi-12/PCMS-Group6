using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Services;
using PCMS_Backend.Shared;
using System.Security.Claims;

namespace PCMS_Backend.Controllers;

[ApiController]
[Authorize (Roles = "Physician")]
[Route("api/physician")]
public class PhysicianController : ControllerBase
{
    private readonly IPhysicianService _physicianService;

    public PhysicianController(IPhysicianService physicianService)
    {
        _physicianService = physicianService;
    }

    [HttpGet("assignments")]
    public async Task<IActionResult> GetAllAssignments()
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(claimValue, out int userId))
        {
            var result = await _physicianService.GetAllAssignmentsAsync(userId);
            return result.ToActionResult();
        }
          return Unauthorized("Invalid or missing session token.");
    }

}

