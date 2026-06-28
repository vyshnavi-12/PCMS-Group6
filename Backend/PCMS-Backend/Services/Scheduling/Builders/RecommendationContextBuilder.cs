using PCMS_Backend.DTOs;
using PCMS_Backend.Services.Scheduling.Interfaces;
using PCMS_Backend.Services.Scheduling.Models;

namespace PCMS_Backend.Services.Scheduling.Builders;

public class RecommendationContextBuilder
    : IRecommendationContextBuilder
{
    private readonly IRecommendationRepository
        _recommendationRepository;

    public RecommendationContextBuilder(
        IRecommendationRepository recommendationRepository)
    {
        _recommendationRepository =
            recommendationRepository;
    }

    public async Task<RecommendationContext> BuildAsync(
        DateOnly coverageDate)
    {
        var physicians =
            await _recommendationRepository
                .GetPhysiciansAsync();

        var leaves =
            await _recommendationRepository
                .GetLeavesAsync();

        var shifts =
            await _recommendationRepository
                .GetExternalShiftsAsync();

        // Only load assignments that can affect
        // rest-gap validation around this assignment.
        var assignmentWindowStart =
            coverageDate.AddDays(-1);

        var assignmentWindowEnd =
            coverageDate.AddDays(1);

        var assignments =
            await _recommendationRepository
                .GetAssignmentsAsync(
                    assignmentWindowStart,
                    assignmentWindowEnd);

        // Historical workload (last 60 days)
        var workloadData =
            await _recommendationRepository
                .GetPhysicianWorkloadsAsync();

        var context = new RecommendationContext
        {
            Physicians = physicians,
            Leaves = leaves,
            ExternalShifts = shifts
        };

        context.AssignmentTracker =
            assignments
                .GroupBy(a => a.PhysicianId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(a =>
                        new AssignmentInfo
                        {
                            CoverageDate = a.CoverageDate,
                            ShiftType = a.ShiftType,
                            SpecialtyId = a.SpecialtyId
                        })
                        .ToList());

        context.Workloads =
            physicians.ToDictionary(
                p => p.PhysicianId,
                p => new PhysicianWorkload());

        // -----------------------------
        // Historical Workload Normalization
        // -----------------------------

        var cutoffDate =
            DateTime.UtcNow.AddDays(-60);

        var oldDoctors =
            workloadData
                .Where(d => d.JoinDate <= cutoffDate)
                .ToList();

        var averageMorning =
            oldDoctors.Any()
                ? (int)oldDoctors
                    .Average(d => d.MorningShiftCount)
                : 0;

        var averageNight =
            oldDoctors.Any()
                ? (int)oldDoctors
                    .Average(d => d.NightShiftCount)
                : 0;

        foreach (var workloadDto in workloadData)
        {
            if (!context.Workloads.TryGetValue(
                    workloadDto.PhysicianId,
                    out var workload))
            {
                continue;
            }

            workload.PastMorningAssignments =
                workloadDto.JoinDate > cutoffDate
                    ? averageMorning
                    : workloadDto.MorningShiftCount;

            workload.PastNightAssignments =
                workloadDto.JoinDate > cutoffDate
                    ? averageNight
                    : workloadDto.NightShiftCount;
        }

        return context;
    }
}