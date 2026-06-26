using System.ComponentModel.DataAnnotations;
namespace PCMS_Backend.Models;

public class CoverageAssignment
{
    
    public int CoverageAssignmentId { get; set; }

    public int CoverageScheduleId { get; set; }

    public DateOnly CoverageDate { get; set; }

    public int SpecialtyId { get; set; }

    public int PhysicianId { get; set; }

    public string ShiftType { get; set; } = default!;

    public string AssignmentStatus { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public CoverageSchedule CoverageSchedule { get; set; } = default!;

    public Specialty Specialty { get; set; } = default!;

    public Physician Physician { get; set; } = default!;

    public ICollection<CoverageGapAlert>? CoverageGapAlerts { get; set; }

    public ICollection<SwapRequest>? RequestedSwapRequests { get; set; }
    public ICollection<SwapRequest>? TargetedSwapRequests { get; set; }
}