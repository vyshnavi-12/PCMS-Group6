using System.ComponentModel.DataAnnotations;

namespace PCMS_Backend.Models;

public class CoverageSchedule
{
    
    public int CoverageScheduleId { get; set; }

    
    public string ScheduleName { get; set; } = default!;

    public DateOnly WeekStartDate { get; set; }

    public DateOnly WeekEndDate { get; set; }

    
    public string Status { get; set; } = default!;

    public int PublishedByUserId { get; set; }

    public DateTime? PublishedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation Properties

    public User PublishedByUser { get; set; } = default!;

    public ICollection<CoverageAssignment> CoverageAssignments { get; set; }
        = default!;
}