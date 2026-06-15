using System.ComponentModel.DataAnnotations;

namespace PCMS_Backend.Models;

public class User
{
    [Key]
    public int UserId { get; set; }

    
    public string EmployeeCode { get; set; } = default!;

    
    public string FullName { get; set; } = default!;

    
    public string EmailAddress { get; set; } = default!;

    
    public string PhoneNumber { get; set; } = default!;

    public string PasswordHash { get; set; } = default!;

    public int RoleId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation Properties

    public Role Role { get; set; } = default!;

    public Physician? Physician { get; set; }

    public ICollection<Notification> Notifications { get; set; }
        = default!;

    public ICollection<AuditLog> AuditLogs { get; set; }
        = default!;

    public ICollection<CoverageSchedule> PublishedSchedules { get; set; }
        = default!;
}