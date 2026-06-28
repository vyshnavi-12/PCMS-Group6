using PCMS_Backend.Services.Scheduling.Models;

namespace PCMS_Backend.Services.Scheduling.Interfaces;

public interface IPhysicianRecommendationService
{
    Task<IReadOnlyList<PhysicianRecommendation>>
        GetRecommendations(
            RecommendationRequest request,
            RecommendationContext context);
    
}