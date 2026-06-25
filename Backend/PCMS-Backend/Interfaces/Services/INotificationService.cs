using PCMS_Backend.Models;
using PCMS_Backend.Shared;

public interface INotificationService
{
    Task<Result<IReadOnlyList<Notification>>> GetUserNotificationsAsync(int userId);
    Task<Result> MarkAsReadAsync(int notificationId, int userId);

    Task CreateSchedulePublishedNotificationsAsync(CoverageSchedule schedule);

    Task CreateAndSendNotificationAsync(
    int userId,
    string title,
    string message
);
}
