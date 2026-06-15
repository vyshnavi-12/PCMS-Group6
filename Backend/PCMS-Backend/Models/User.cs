using Physician_On_Call_Schedule_Management_System.Models;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace PCMS_Backend.Models;

public class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(20)]
    public string EmployeeCode { get; set; } = default!;

    [StringLength(100)]
    public string FullName { get; set; } = default!;

    [StringLength(150)]
    public string EmailAddress { get; set; } = default!;

    [StringLength(15)]
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