using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Models;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Services;

public class CoverageAssignmentsService : ICoverageAssignmentsService
{
    private readonly ICoverageAssignmentsRepository _coverageAssignmentsRepo;
    private readonly IPhysicianService _physicianService;


    public CoverageAssignmentsService(ICoverageAssignmentsRepository coverageAssignmentsRepo, IPhysicianService physicianService)
    {
        _coverageAssignmentsRepo = coverageAssignmentsRepo;
        _physicianService = physicianService;
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

        await _coverageAssignmentsRepo.CreateAlertAsync(gapAlert, physicianId);

        return Result.Ok("Assignment marked unavailable. Alert sent to supervisor.");
    }

    public async Task<Result<IReadOnlyList<OpenAlertsResponseDto>>> GetAlertsAsync()
    {
        var openAlerts = await _coverageAssignmentsRepo.GetAlertsAsync();
        return Result<IReadOnlyList<OpenAlertsResponseDto>>.Ok(openAlerts);
    }

    public async Task<Result<AlertDetailsResponseDto>> GetAlertDetailsAsync(int alertId)
    {
        var alert = await _coverageAssignmentsRepo.GetAlertDetailsByIdAsync(alertId);

        if (alert == null)
            return Result<AlertDetailsResponseDto>.NotFound("Alert not found.");

        // Run the workload algorithm to get replacements
        var replacementsResult = await _physicianService.GetSuggestedReplacementsAsync(alert.CoverageAssignmentId);

        var replacementsList = replacementsResult.Data?.ToList() ?? new List<ReplacementPhysicianDto>();
        // Map only the data the modal actually needs
        var response = new AlertDetailsResponseDto
        {
            Reason = alert.AlertReason ?? "No reason provided",
            Replacements = replacementsList
        };

        return Result<AlertDetailsResponseDto>.Ok(response, "Alert details fetched successfully.");
    }
    public async Task<Result> UpdateAlertPhysicianAsync(int alertId, int physicianId)
    {
        var alert = await _coverageAssignmentsRepo.GetAlertDetailsByIdAsync(alertId);
        if (alert == null)
            return Result.NotFound("Alert not found.");
        var updateAssignment = await _coverageAssignmentsRepo.UpdateAssignmentPhysicianAsync(alertId, physicianId);
        var updateAlertStatus = await _coverageAssignmentsRepo.UpdateAlertStatusToResolvedAsync(alertId);
        if(!updateAssignment || !updateAlertStatus) return Result.ServerError("Failed to update assignment or resolve alert.");
        return Result.Ok("Assignment updated with new physician and alert resolved.");
    }

    public async Task<Result> DeclineUnavailableRequestAsync(int alertId)
    {
        var declineAlertDone = await _coverageAssignmentsRepo.UpdateAlertStatusToResolvedAsync(alertId);
        if (!declineAlertDone) return Result.ServerError("Failed to decline alert.");
        return Result.NoContent();
    }

    public async Task<Result<IReadOnlyList<UnavailableRequestsPerSpecialtyDto>>> GetUnavailableRequestsPerSpecialtyAsync()
    {
        var data =  await _coverageAssignmentsRepo.GetUnavailableRequestsPerSpecialtyAsync();
        if (data == null) return Result<IReadOnlyList<UnavailableRequestsPerSpecialtyDto>>.ServerError("Failed to fetch data");
        return Result<IReadOnlyList<UnavailableRequestsPerSpecialtyDto>>.Ok(data);
    }

}