using Microsoft.AspNetCore.SignalR;
using PCMS_Backend.Models;
using PCMS_Backend.Services;
using PCMS_Backend.Shared;
using PCMS_Backend.Hubs;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IAuditLogService _auditLogService; // added by me


    public NotificationService(INotificationRepository repository, IAuditLogService auditLogService, IHubContext<NotificationHub> hubContext)
    {
        _repository = repository;
        _auditLogService = auditLogService; // added by me
        _hubContext = hubContext;
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
    public async Task CreateSchedulePublishedNotificationsAsync(CoverageSchedule schedule)
    {
        var userIds = schedule.CoverageAssignments
            .Select(ca => ca.Physician.UserId)
            .Distinct()
            .ToList();

        foreach (var userId in userIds)
        {
            if (userId != null)
            {
                await CreateAndSendNotificationAsync(
                    (int)userId,
                    "On-Call Schedule Published",
                    $"You have been assigned on-call duties in '{schedule.ScheduleName}'. Please review your schedule."
                );
            }
        }
    }

    public async Task CreateAndSendNotificationAsync(
    int userId,
    string title,
    string message)
    {
        var notification = new Notification
        {
            UserId = userId,
            NotificationTitle = title,
            NotificationMessage = message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.CreateAsync(notification);

        await _hubContext.Clients
            .Group($"User_{userId}")
            .SendAsync("ReceiveNotification", new
            {
                notificationId = notification.NotificationId,
                title = notification.NotificationTitle,
                message = notification.NotificationMessage,
                createdAt = notification.CreatedAt
            });
    }
}
