using PCMS_Backend.Models;
using PCMS_Backend.Services.Scheduling.Interfaces;
using PCMS_Backend.Services.Scheduling.Models;
using PCMS_Backend.Services.Scheduling.Rules;
using PCMS_Backend.Services.Scheduling.Scoring;

namespace PCMS_Backend.Services.Scheduling.Engines;

public class PhysicianRecommendationService
    : IPhysicianRecommendationService
{
    private readonly IEnumerable<IEligibilityRule> _rules;

    private readonly IFairnessScorer _fairnessScorer;

    public PhysicianRecommendationService(
        IEnumerable<IEligibilityRule> rules,
        IFairnessScorer fairnessScorer)
    {
        _rules = rules;
        _fairnessScorer = fairnessScorer;
    }

    public  Task<IReadOnlyList<PhysicianRecommendation>> GetRecommendations(
            RecommendationRequest request,
            RecommendationContext context)
    {
        var eligiblePhysicians = context.Physicians
           .Where(p =>
                request.ExcludePhysicianId == null ||
                p.PhysicianId != request.ExcludePhysicianId)
        .Where(physician =>
            _rules.All(rule =>
                rule.IsEligible(
                    physician,
                    request,
                    context)))
        .ToList();

        var primaryCandidates = new List<(Physician Physician, bool IsPrimary)>();
        var secondaryCandidates = new List<(Physician Physician, bool IsPrimary)>();

        foreach (var physician in eligiblePhysicians)
        {
            var specialty = physician.PhysicianSpecialtyMaps
                .FirstOrDefault(ps => ps.SpecialtyId == request.SpecialtyId);

            if (specialty == null)
                continue;

            if (specialty.IsPrimarySpecialty)
            {
                primaryCandidates.Add((physician, true));
            }
            else
            {
                secondaryCandidates.Add((physician, false));
            }
        }

        var primaryRecommendations = primaryCandidates
     .Select(c => new PhysicianRecommendation
     {
         PhysicianId = c.Physician.PhysicianId,
         PhysicianName = c.Physician.User.FullName,
         Score = _fairnessScorer.CalculateScore(c.Physician, request, context),
         IsPrimarySpecialty = c.IsPrimary
     })
     .OrderBy(r => r.Score);

        var secondaryRecommendations = secondaryCandidates
            .Select(c => new PhysicianRecommendation
            {
                PhysicianId = c.Physician.PhysicianId,
                PhysicianName = c.Physician.User.FullName,
                Score = _fairnessScorer.CalculateScore(c.Physician, request, context),
                IsPrimarySpecialty = c.IsPrimary
            })
            .OrderBy(r => r.Score);

        // Combine: primary first, then secondary, then take top 10
        var recommendations = primaryRecommendations
            .Concat(secondaryRecommendations)
            .Take(10)
            .ToList();


        return Task.FromResult<IReadOnlyList<PhysicianRecommendation>>(recommendations);
    }

    
}