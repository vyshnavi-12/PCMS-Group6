using PCMS_Backend.Models;
using PCMS_Backend.Services.Scheduling.Constants;
using PCMS_Backend.Services.Scheduling.Models;

namespace PCMS_Backend.Services.Scheduling.Rules;

public class WeeklyLimitEligibilityRule
    : IEligibilityRule
{
    public bool IsEligible(
        Physician physician,
        RecommendationRequest request,
        RecommendationContext context)
    {
        // Schedule generation fallback mode.
        // When enabled, weekly limits are ignored
        // and the fairness scorer will pick the
        // least-loaded physician.
        if (request.AllowLimitOverride)
        {
            return true;
        }

        if (!context.Workloads.TryGetValue(
                physician.PhysicianId,
                out var workload))
        {
            return true;
        }

        if (workload.CurrentMorningAssignments+workload.CurrentNightAssignments >=
            SchedulingRules.MaxAssignmentsPerSchedule)
        {
            return false;
        }

        if (string.Equals(
                request.ShiftType,
                "Night",
                StringComparison.OrdinalIgnoreCase)
            &&
            workload.CurrentNightAssignments >=
            SchedulingRules.MaxNightAssignmentsPerSchedule)
        {
            return false;
        }

        return true;
    }
}