using PCMS_Backend.Models;
using PCMS_Backend.Services.Scheduling.Models;

namespace PCMS_Backend.Services.Scheduling.Rules;

public class LeaveEligibilityRule : IEligibilityRule
{
    public bool IsEligible(
        Physician physician,
        RecommendationRequest request,
        RecommendationContext context)
    {
        return !context.Leaves.Any(l =>
            l.PhysicianId == physician.PhysicianId &&
            request.CoverageDate >= l.LeaveStartDate &&
            request.CoverageDate <= l.LeaveEndDate);
    }
}