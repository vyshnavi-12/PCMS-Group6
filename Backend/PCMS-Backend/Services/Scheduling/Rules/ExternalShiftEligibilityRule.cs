using PCMS_Backend.Models;
using PCMS_Backend.Services.Scheduling.Models;

namespace PCMS_Backend.Services.Scheduling.Rules;

public class ExternalShiftEligibilityRule
    : IEligibilityRule
{
    public bool IsEligible(
        Physician physician,
        RecommendationRequest request,
        RecommendationContext context)
    {
        return !context.ExternalShifts.Any(es =>
            es.PhysicianId == physician.PhysicianId &&
            DateOnly.FromDateTime(es.StartTime)
                <= request.CoverageDate &&
            DateOnly.FromDateTime(es.EndTime)
                >= request.CoverageDate);
    }
}