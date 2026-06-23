using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Services;
using System.Security.Claims;

namespace PCMS_Backend.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SwapRequestsController : ControllerBase
{
    private readonly ISwapRequestService _service;

    public SwapRequestsController(
        ISwapRequestService service
    )
    {
        _service = service;
    }

    [HttpGet("available-targets/{coverageAssignmentId}")]
    public async Task<IActionResult> GetAvailableTargets(
        int coverageAssignmentId
    )
    {
        var result = await _service
            .GetAvailableTargetsAsync(coverageAssignmentId);

        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSwapRequest(
        [FromBody] CreateSwapRequestDto dto
    )
    {
        var claimValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (!int.TryParse(claimValue, out int userId))
            return Unauthorized();

        var result = await _service
            .CreateSwapRequestAsync(userId, dto);

        return StatusCode(result.StatusCode ?? 500, result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyRequests()
    {
        var claimValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (!int.TryParse(claimValue, out int userId))
            return Unauthorized();

        var result = await _service
            .GetMyRequestsAsync(userId);

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("to-me")]
    public async Task<IActionResult> GetRequestsToMe()
    {
        var claimValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (!int.TryParse(claimValue, out int userId))
            return Unauthorized();

        var result = await _service
            .GetRequestsToMeAsync(userId);

        return StatusCode(result.StatusCode, result);
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

    [HttpPut("{id}/decline")]
    public async Task<IActionResult> DeclineRequest(int id)
    {
        var claimValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (!int.TryParse(claimValue, out int userId))
            return Unauthorized();

        var result = await _service
            .DeclineRequestAsync(id, userId);

        return StatusCode(result.StatusCode ?? 500, result);
    }

    [HttpGet("supervisor")]
    public async Task<IActionResult> GetSupervisorRequests()
    {
        var result = await _service.GetSupervisorRequestsAsync();
        return StatusCode(result.StatusCode, result);
    }
}