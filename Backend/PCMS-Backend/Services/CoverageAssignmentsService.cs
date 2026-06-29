using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Models;
using PCMS_Backend.Services.Scheduling.Interfaces;
using PCMS_Backend.Services.Scheduling.Models;
using PCMS_Backend.Shared;
using Microsoft.AspNetCore.SignalR;
using PCMS_Backend.Hubs;

namespace PCMS_Backend.Services;

public class CoverageAssignmentsService : ICoverageAssignmentsService
{
    private readonly ICoverageAssignmentsRepository _coverageAssignmentsRepo;
    private readonly IPhysicianService _physicianService;
    private readonly IPhysicianRecommendationService _physicianRecommendationService;
    private readonly IRecommendationContextBuilder _contextBuilder;
    private readonly IHubContext<UnavailableRequestHub> _hubContext;



    public CoverageAssignmentsService(
    ICoverageAssignmentsRepository coverageAssignmentsRepo,
    IPhysicianService physicianService,
    IPhysicianRecommendationService physicianRecommendationService,
    IRecommendationContextBuilder contextBuilder,
    IHubContext<UnavailableRequestHub> hubContext)
    {
        _coverageAssignmentsRepo = coverageAssignmentsRepo;
        _physicianService = physicianService;
        _physicianRecommendationService = physicianRecommendationService;
        _contextBuilder = contextBuilder;
        _hubContext = hubContext;
    }

    public async Task<Result> MarkAssignmentUnavailableAsync(int assignmentId, string reason, int physicianId)
    {
        var assignment = await _coverageAssignmentsRepo.GetAssignmentByIdAsync(assignmentId);

        if (assignment == null)
            return Result.NotFound("Assignment not found.");

        if (assignment.PhysicianId != physicianId)
            return Result.Forbidden("You are not authorized to modify this assignment.");

        var gapAlert = new CoverageGapAlertDto
        {
            CoverageAssignmentId = assignmentId,
            CreatedAt = DateTime.UtcNow,
            AlertReason = reason,
            AlertStatus = "Open"
        };
        await _coverageAssignmentsRepo.ChangeAssignmentStatus(assignmentId, "Pending");
        await _coverageAssignmentsRepo.CreateAlertAsync(gapAlert, physicianId);

        await _hubContext.Clients
            .Group("User_6")
            .SendAsync("NewUnavailableRequest");

        return Result.Ok("Assignment marked unavailable. Alert sent to supervisor.");
    }

    public async Task<Result<IReadOnlyList<OpenAlertsResponseDto>>> GetAlertsAsync()
    {
        var openAlerts = await _coverageAssignmentsRepo.GetAlertsAsync();
        return Result<IReadOnlyList<OpenAlertsResponseDto>>.Ok(openAlerts);
    }

    public async Task<Result<AlertDetailsResponseDto>> GetAlertDetailsAsync(int alertId)
    {
        var alert =
            await _coverageAssignmentsRepo
                .GetAlertDetailsByIdAsync(alertId);

        if (alert == null)
        {
            return Result<AlertDetailsResponseDto>
                .NotFound("Alert not found.");
        }

        // Load the assignment referenced by the alert
        var assignment =
            await _coverageAssignmentsRepo
                .GetAssignmentByIdAsync(
                    alert.CoverageAssignmentId);

        if (assignment == null)
        {
            return Result<AlertDetailsResponseDto>
                .NotFound("Coverage assignment not found.");
        }

        var context =
            await _contextBuilder.BuildAsync(
                assignment.CoverageDate);

        var request =
            new RecommendationRequest
            {
                CoverageDate = assignment.CoverageDate,
                ShiftType = assignment.ShiftType,
                SpecialtyId = assignment.SpecialtyId,
                ExcludePhysicianId = assignment.PhysicianId
            };

        var recommendations =
            await _physicianRecommendationService
                .GetRecommendations(
                    request,
                    context);

        var replacements =
            recommendations
                .Select((r,index) => new ReplacementPhysicianDto
                {
                    PhysicianId = r.PhysicianId,
                    PhysicianName = r.PhysicianName,
                    IsRecommended=index==0
                    
                })
                .ToList();

        var response =
            new AlertDetailsResponseDto
            {
                Reason = alert.AlertReason ?? "No reason provided",
                Replacements = replacements
            };

        return Result<AlertDetailsResponseDto>.Ok(
            response,
            "Alert details fetched successfully.");
    }

    public async Task<Result> UpdateAlertPhysicianAsync(int alertId, int physicianId)
    {
        var alert = await _coverageAssignmentsRepo.GetAlertDetailsByIdAsync(alertId);
        if (alert == null)
            return Result.NotFound("Alert not found.");

        var updateAssignment = await _coverageAssignmentsRepo.UpdateAssignmentPhysicianAsync(alertId, physicianId);
        var assignmentId = await _coverageAssignmentsRepo.GetAssignmentIdByAlertIdAsync(alertId);
        if (assignmentId is not int validAssignmentId) return Result.NotFound("Assignment not found");
        await _coverageAssignmentsRepo.ChangeAssignmentStatus(validAssignmentId, "Active");
        var updateAlertStatus = await _coverageAssignmentsRepo.UpdateAlertStatusToResolvedAsync(alertId);
        if (!updateAssignment || !updateAlertStatus)
            return Result.ServerError("Failed to update assignment or resolve alert.");

        var userId = alert.Physician.UserId;

        if (userId != null)
        {
            await _hubContext.Clients
                .Group($"User_{userId}")
                .SendAsync("UnavailableRequestUpdated");
        }

        return Result.Ok("Assignment updated with new physician and alert resolved.");
    }

    public async Task<Result> DeclineUnavailableRequestAsync(int alertId)
    {
        var alert = await _coverageAssignmentsRepo.GetAlertDetailsByIdAsync(alertId);

        if (alert == null)
            return Result.NotFound("Alert not found");

        var assignmentId = await _coverageAssignmentsRepo.GetAssignmentIdByAlertIdAsync(alertId);

        if (assignmentId is not int validAssignmentId)
            return Result.NotFound("Assignment not found");

        await _coverageAssignmentsRepo.ChangeAssignmentStatus(validAssignmentId, "Active");

        var declineAlertDone = await _coverageAssignmentsRepo.UpdateAlertStatusToResolvedAsync(alertId);

        if (!declineAlertDone)
            return Result.ServerError("Failed to decline alert.");

        var userId = alert.Physician.UserId;

        if (userId != null)
        {
            await _hubContext.Clients
                .Group($"User_{userId}")
                .SendAsync("UnavailableRequestUpdated");
        }

        return Result.NoContent();
    }

    public async Task<Result<IReadOnlyList<UnavailableRequestsPerSpecialtyDto>>> GetUnavailableRequestsPerSpecialtyAsync()
    {
        var data =  await _coverageAssignmentsRepo.GetUnavailableRequestsPerSpecialtyAsync();
        if (data == null) return Result<IReadOnlyList<UnavailableRequestsPerSpecialtyDto>>.ServerError("Failed to fetch data");
        return Result<IReadOnlyList<UnavailableRequestsPerSpecialtyDto>>.Ok(data);
    }

    public async Task<Result<IReadOnlyList<ReplacementPhysicianDto>>>
        GetRecommendationsAsync(
            int assignmentId)
            {
                var assignment =
                    await _coverageAssignmentsRepo
                        .GetAssignmentByIdAsync(
                            assignmentId);

                if (assignment == null)
                {
                    return Result<IReadOnlyList<ReplacementPhysicianDto>>
                        .NotFound("Assignment not found.");
                }

                var context =
                    await _contextBuilder.BuildAsync(
                        assignment.CoverageDate);

                var request =
                    new RecommendationRequest
                    {
                        CoverageDate = assignment.CoverageDate,
                        ShiftType = assignment.ShiftType,
                        SpecialtyId = assignment.SpecialtyId,
                        ExcludePhysicianId = assignment.PhysicianId
                    };

                var recommendations =
                    await _physicianRecommendationService
                        .GetRecommendations(
                            request,
                            context);

                var replacements =
                    recommendations
                        .Select((r, index) => new ReplacementPhysicianDto
                        {
                            PhysicianId = r.PhysicianId,
                            PhysicianName = r.PhysicianName,
                            IsRecommended = index == 0
                        })
                        .ToList();

                return Result<IReadOnlyList<ReplacementPhysicianDto>>
                    .Ok(replacements);
            }

}