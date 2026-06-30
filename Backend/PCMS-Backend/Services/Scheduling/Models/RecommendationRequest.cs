namespace PCMS_Backend.Services.Scheduling.Models;

public class RecommendationRequest
{
    public int CoverageAssignmentId { get; set; }
    public DateOnly CoverageDate { get; set; }

    public string ShiftType { get; set; } = default!;

    public int SpecialtyId { get; set; }

    public int? ExcludePhysicianId { get; set; }

    // Used during schedule generation when no physician
    // satisfies weekly limits. Allows generator to fall
    // back to the least-loaded physician.
    public bool AllowLimitOverride { get; set; } = false;
}