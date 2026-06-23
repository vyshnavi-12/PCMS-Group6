using PCMS_Backend.Models;

public interface INotificationRepository
{
    Task<IReadOnlyList<Notification>> GetByUserIdAsync(int userId);
    Task<Notification?> GetByIdAsync(int id);
    Task UpdateAsync(Notification notification);

    Task CreateAsync(Notification notification);
    Task AddRangeAsync(List<Notification> notifications);

}