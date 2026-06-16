using System.ComponentModel.DataAnnotations;
namespace PCMS_Backend.Models;

public class CoverageGapAlert
{
    
    public int CoverageGapAlertId { get; set; }

    public int CoverageAssignmentId { get; set; }

    public int? SuggestedReplacementPhysicianId { get; set; }

    public string AlertStatus { get; set; } = default!;

    public string AlertReason { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public CoverageAssignment CoverageAssignment { get; set; } = default!;

    public Physician? SuggestedReplacementPhysician { get; set; }
}