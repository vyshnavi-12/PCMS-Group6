using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Shared;
using System.Security.Claims;

namespace PCMS_Backend.Controllers;

[Authorize(Roles = "Physician")]
[Route("api/[controller]")]
[ApiController]
public class MyScheduleController : ControllerBase
{
    private readonly IMyScheduleService _service;

    public MyScheduleController(IMyScheduleService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetMySchedule()
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        var result = await _service.GetMyScheduleAsync(userId);

        return result.ToActionResult();
    }
}