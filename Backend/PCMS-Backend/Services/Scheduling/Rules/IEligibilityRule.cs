using PCMS_Backend.Models;
using PCMS_Backend.Services.Scheduling.Models;

namespace PCMS_Backend.Services.Scheduling.Rules
{
    public interface IEligibilityRule
    {
        bool IsEligible(
            Physician physician,
            RecommendationRequest request,
            RecommendationContext context);
    }
}
