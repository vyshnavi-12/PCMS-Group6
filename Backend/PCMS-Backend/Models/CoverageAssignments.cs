using System.ComponentModel.DataAnnotations;
namespace PCMS_Backend.Models;

public class CoverageAssignments
{
    [Key]
    public int CoverageAssignmentId { get; set; }

    public int CoverageScheduleId { get; set; }

    public DateOnly CoverageDate { get; set; }

    public int SpecialtyId { get; set; }

    public int PhysicianId { get; set; }

    public string ShiftType { get; set; } = default!;

    public string AssignmentStatus { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public CoverageSchedule CoverageSchedule { get; set; }

    public Specialty Specialty { get; set; }

    public Physicians Physicians { get; set; }

    public ICollection<CoverageGapAlerts>? CoverageGapAlerts { get; set; }

    public ICollection<SwapRequests>? SwapRequests { get; set; }
}