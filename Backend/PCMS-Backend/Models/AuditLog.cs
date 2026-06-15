using System.ComponentModel.DataAnnotations;

namespace PCMS_Backend.Models;

public class AuditLog
{
    [Key]
    public int AuditLogId { get; set; }

    
    public string ActionType { get; set; } = default!;

    
    public string EntityName { get; set; } = default!;

    public int EntityRecordId { get; set; }

    public int PerformedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation Property

    public User PerformedByUser { get; set; } = default!;
}