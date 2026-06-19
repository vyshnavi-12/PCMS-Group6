using PCMS_Backend.Models;
using PCMS_Backend.Shared;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;

    public NotificationService(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<Notification>>> GetUserNotificationsAsync(int userId)
    {
        var notifications = await _repository.GetByUserIdAsync(userId);

       
        return Result<IReadOnlyList<Notification>>.Ok(notifications, notifications.Count == 0
            ? "No notifications found."
            : "Notifications retrieved successfully");
    }

    public async Task<Result> MarkAsReadAsync(int notificationId, int userId)
    {
        var notification = await _repository.GetByIdAsync(notificationId);
        if (notification == null)
            return Result.NotFound("Notification not found");
        else if(notification.UserId != userId)
            return Result.Forbidden("Notification not owned by user.");

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        await _repository.UpdateAsync(notification);

        return Result.Ok("Notification marked as read.");
    }
}
