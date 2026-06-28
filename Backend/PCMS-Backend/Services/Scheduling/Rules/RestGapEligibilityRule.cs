using PCMS_Backend.Models;
using PCMS_Backend.Services.Scheduling.Models;

namespace PCMS_Backend.Services.Scheduling.Rules;

public class RestGapEligibilityRule
    : IEligibilityRule
{
    public bool IsEligible(
        Physician physician,
        RecommendationRequest request,
        RecommendationContext context)
    {
        if (!context.AssignmentTracker.TryGetValue(
                physician.PhysicianId,
                out var assignments))
        {
            return true;
        }

        foreach (var assignment in assignments)
        {
            // Same day
            if (assignment.CoverageDate ==
                request.CoverageDate)
            {
                return false;
            }

            // Night -> Next Day Day
            if (assignment.ShiftType == "Night" &&
                request.ShiftType == "Day" &&
                assignment.CoverageDate.AddDays(1)
                    == request.CoverageDate)
            {
                return false;
            }

            // Previous Day Night <- Day
            if (assignment.ShiftType == "Day" &&
                request.ShiftType == "Night" &&
                request.CoverageDate.AddDays(1)
                    == assignment.CoverageDate)
            {
                return false;
            }
        }

        return true;
    }
}