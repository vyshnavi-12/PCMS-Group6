using PCMS_Backend.Models;
using PCMS_Backend.Services.Scheduling.Models;

namespace PCMS_Backend.Services.Scheduling.Scoring;

public class FairnessScorer : IFairnessScorer
{
    public int CalculateScore(
        Physician physician,
        RecommendationRequest request,
        RecommendationContext context)
    {
        if (!context.Workloads.TryGetValue(
                physician.PhysicianId,
                out var workload))
        {
            return 0;
        }

        return
            (2 * workload.PastMorningAssignments) +
            (3 * workload.PastNightAssignments) +
            (4 * workload.CurrentMorningAssignments) +
            (6 * workload.CurrentNightAssignments);
    }
}