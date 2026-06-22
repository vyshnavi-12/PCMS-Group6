using Microsoft.AspNetCore.Mvc;
using PCMS_Backend.Interfaces.Services;

namespace PCMS_Backend.Controllers;

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
}