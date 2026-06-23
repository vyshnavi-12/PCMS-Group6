using Microsoft.EntityFrameworkCore;
using PCMS_Backend.Data;
using PCMS_Backend.Models;

public class NotificationRepository : INotificationRepository
{
    private readonly PcmsDbContext _context;

    public NotificationRepository(PcmsDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Notification>> GetByUserIdAsync(int userId)
    {
        return await _context.Notifications
                             .Where(n => n.UserId == userId)
                             .OrderByDescending(n => n.CreatedAt)
                             .ToListAsync();
    }

    public async Task<Notification?> GetByIdAsync(int id)
    {
        return await _context.Notifications.FirstOrDefaultAsync(n => n.NotificationId == id);
    }

    public async Task UpdateAsync(Notification notification)
    {
        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync();
    }

    public async Task CreateAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
    }
}
