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
        if (notifications.Count == 0)
            return Result<IReadOnlyList<Notification>>.NotFound("No notifications found.");

        return Result<IReadOnlyList<Notification>>.Ok(notifications, "Notifications retrieved successfully");
    }

    public async Task<Result> MarkAsReadAsync(int notificationId, int userId)
    {
        var notification = await _repository.GetByIdAsync(notificationId);
        if (notification == null || notification.UserId != userId)
            return Result.NotFound("Notification not found or not owned by user.");

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        await _repository.UpdateAsync(notification);

        return Result.Ok("Notification marked as read.");
    }
}
