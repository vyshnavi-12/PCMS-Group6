using PCMS_Backend.Services.Scheduling.Models;

namespace PCMS_Backend.Services.Scheduling.Interfaces;

public interface IRecommendationContextBuilder
{
    Task<RecommendationContext> BuildAsync(
        DateOnly startDate);
}