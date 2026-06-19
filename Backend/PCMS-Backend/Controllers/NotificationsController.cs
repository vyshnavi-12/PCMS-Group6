using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCMS_Backend.Shared;
using System.Security.Claims;

[Authorize] // ✅ Only authenticated users can access
[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;

    public NotificationsController(INotificationService service)
    {
        _service = service;
    }

    // GET /api/notifications
    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(claimValue, out int userId))
        {
// ✅ Pass both logged-in userId and requested userId (same in this case)
            var result = await _service.GetUserNotificationsAsync(userId);
        return result.ToActionResult();
        }
        return Unauthorized("Invalid or missing session token.");

    }

    // POST /api/notifications/{id}/mark-read
    [HttpPost("{id}/mark-read")]
    public async Task<IActionResult> MarkNotificationAsRead(int id)
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(claimValue, out int userId))
        { 
         // ✅ Pass both logged-in userId and requested userId (same in this case)
            var result = await _service.MarkAsReadAsync(id, userId);
            return result.ToActionResult();
        }
        return Unauthorized("Invalid or missing session token.");
    }



}
