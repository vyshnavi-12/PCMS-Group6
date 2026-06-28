using PCMS_Backend.Models;
using PCMS_Backend.Services.Scheduling.Models;

namespace PCMS_Backend.Services.Scheduling.Rules;

public class SpecialtyEligibilityRule
    : IEligibilityRule
{
    public bool IsEligible(
        Physician physician,
        RecommendationRequest request,
        RecommendationContext context)
    {
        return physician.PhysicianSpecialtyMaps
            .Any(ps =>
                ps.SpecialtyId ==
                request.SpecialtyId);
    }
}