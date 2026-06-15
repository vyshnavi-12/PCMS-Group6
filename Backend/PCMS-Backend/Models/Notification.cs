using System.ComponentModel.DataAnnotations;

namespace PCMS_Backend.Models;

public class Notification
{
    [Key]
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    
    public string NotificationTitle { get; set; } = default!;

    
    public string NotificationMessage { get; set; } = default!;

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ReadAt { get; set; }

    // Navigation Property

    public User User { get; set; } = default!;
}