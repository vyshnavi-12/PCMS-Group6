using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCMS_Backend.Shared;

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
        int userId = int.Parse(User.FindFirst("UserId")!.Value);
        var result = await _service.GetUserNotificationsAsync(userId);
        return result.ToActionResult();
    }

    // POST /api/notifications/{id}/mark-read
    [HttpPost("{id}/mark-read")]
    public async Task<IActionResult> MarkNotificationAsRead(int id)
    {
        int userId = int.Parse(User.FindFirst("UserId")!.Value);
        var result = await _service.MarkAsReadAsync(id, userId);
        return result.ToActionResult();
    }
}
