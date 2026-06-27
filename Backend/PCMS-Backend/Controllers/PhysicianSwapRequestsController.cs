using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Shared;
using System.Security.Claims;

namespace PCMS_Backend.Controllers;

[ApiController]
[Authorize(Roles = "Physician")]
[Route("api/Physician/SwapRequests")]

public class PhysicianSwapRequestsController : ControllerBase
{
    private readonly ISwapRequestService _service;

    public PhysicianSwapRequestsController(
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

        if (User.GetCurrentUserId() is not int validUserId) return Unauthorized("Invalid session token.");

       

        

        var result = await _service
            .CreateSwapRequestAsync(validUserId, dto);

        return StatusCode(result.StatusCode ?? 500, result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyRequests()
    {
        if (User.GetCurrentUserId() is not int validUserId) return Unauthorized("Invalid session token.");



        var result = await _service
            .GetMyRequestsAsync(validUserId);

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("to-me")]
    public async Task<IActionResult> GetRequestsToMe()
    {
        if (User.GetCurrentUserId() is not int validUserId) return Unauthorized("Invalid session token.");


        

        var result = await _service
            .GetRequestsToMeAsync(validUserId);

        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}/accept")]
    public async Task<IActionResult> AcceptRequest(int id)
    {
        if (User.GetCurrentUserId() is not int validUserId)
            return Unauthorized("Invalid session token.");

        var result = await _service.AcceptRequestAsync(id, validUserId);

        return StatusCode(result.StatusCode ?? 500, result);
    }

    [HttpPut("{id}/decline")]
    public async Task<IActionResult> DeclineRequest(int id)
    {
        if (User.GetCurrentUserId() is not int validUserId) return Unauthorized("Invalid session token.");


        

        var result = await _service
            .DeclineRequestAsync(id, validUserId);

        return StatusCode(result.StatusCode ?? 500, result);
    }

    [HttpGet("pending-my-count")]
    public async Task<IActionResult> GetPendingMyRequestsCount()
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(claimValue, out int userId))
            return Unauthorized();

        var result = await _service.GetPendingMyRequestsCountAsync(userId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("pending-to-me-count")]
    public async Task<IActionResult> GetPendingRequestsToMeCount()
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(claimValue, out int userId))
            return Unauthorized();

        var result = await _service.GetPendingRequestsToMeCountAsync(userId);
        return StatusCode(result.StatusCode, result);
    }



}