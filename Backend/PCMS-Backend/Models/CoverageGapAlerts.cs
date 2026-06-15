using System.ComponentModel.DataAnnotations;
namespace PCMS_Backend.Models;


public class CoverageGapAlerts
{
    [Key]
    public int CoverageGapAlertId { get; set; }

    public int CoverageAssignmentId { get; set; }

    public int? SuggestedReplacementPhysicianId { get; set; }

    public string? AlertStatus { get; set; }

    public string? AlertReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public CoverageAssignments CoverageAssignment { get; set; } = default!;

    public Physicians? SuggestedReplacementPhysician { get; set; }
}