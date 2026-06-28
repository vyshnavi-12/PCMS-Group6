using PCMS_Backend.Models;

namespace PCMS_Backend.Services.Scheduling.Models;

public class RecommendationContext
{
    public IReadOnlyList<Physician> Physicians { get; set; }
        = [];

    public IReadOnlyList<ExternalLeavesData> Leaves { get; set; }
        = [];

    public IReadOnlyList<ExternalShiftsData> ExternalShifts { get; set; }
        = [];

    public Dictionary<int, PhysicianWorkload> Workloads { get; set; }
        = [];

    public Dictionary<int, List<AssignmentInfo>> AssignmentTracker { get; set; }
        = [];
}