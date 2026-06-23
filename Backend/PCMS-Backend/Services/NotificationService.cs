using PCMS_Backend.Models;
using PCMS_Backend.Services;
using PCMS_Backend.Shared;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;
    private readonly IAuditLogService _auditLogService; // added by me


    public NotificationService(INotificationRepository repository, IAuditLogService auditLogService)
    {
        _repository = repository;
        _auditLogService = auditLogService; // added by me
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

        // added by me
        await _auditLogService.LogActionAsync(
            "NotificationRead",
            "Notification",
            notification.NotificationId,
            userId
        );


        return Result.Ok("Notification marked as read.");
    }
    public async Task CreateSchedulePublishedNotificationsAsync(
    CoverageSchedule schedule)
    {
        var notifications = schedule.CoverageAssignments
            .Select(ca => ca.Physician.UserId)
            .Distinct()
            .Select(userId => new Notification
            {
                UserId =(int) userId,
                NotificationTitle = "On-Call Schedule Published",
                NotificationMessage =
                    $"You have been assigned on-call duties in '{schedule.ScheduleName}'. Please review your schedule.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            })
            .ToList();

        await _repository.AddRangeAsync(notifications);
    }
}
