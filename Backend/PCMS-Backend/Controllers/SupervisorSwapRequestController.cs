using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Services;
using System.Security.Claims;

namespace PCMS_Backend.Controllers;

[Authorize(Roles = "Supervisor")]
[Route("api/Supervisor/SwapRequests")]
[ApiController]
public class SupervisorSwapRequestsController : ControllerBase
{
    private readonly ISwapRequestService _service;

    public SupervisorSwapRequestsController(
        ISwapRequestService service
    )
    {
        _service = service;
    }
    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveRequest(int id)
    {
        var claimValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (!int.TryParse(claimValue, out int userId))
            return Unauthorized();

        var result = await _service
            .ApproveRequestAsync(id, userId);

        return StatusCode(result.StatusCode ?? 500, result);
    }

    [HttpPut("{id}/reject")]
    public async Task<IActionResult> RejectRequest(int id)
    {
        var claimValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (!int.TryParse(claimValue, out int userId))
            return Unauthorized();

        var result = await _service
            .RejectRequestAsync(id, userId);

        return StatusCode(result.StatusCode ?? 500, result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetSupervisorRequests()
    {
        var result = await _service.GetSupervisorRequestsAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("target-accepted-count")]
    public async Task<IActionResult> GetTargetAcceptedCount()
    {
        var result = await _service.GetTargetAcceptedCountAsync();
        return StatusCode(result.StatusCode, result);
    }

}