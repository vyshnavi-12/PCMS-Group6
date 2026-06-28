using PCMS_Backend.Models;
using PCMS_Backend.Services.Scheduling.Models;

namespace PCMS_Backend.Services.Scheduling.Scoring;

public interface IFairnessScorer
{
    int CalculateScore(
        Physician physician,
        RecommendationRequest request,
        RecommendationContext context);
}