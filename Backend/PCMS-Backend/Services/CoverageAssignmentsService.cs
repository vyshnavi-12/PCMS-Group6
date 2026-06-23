using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Models;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Services;

public class CoverageAssignmentsService : ICoverageAssignmentsService
{
    private readonly ICoverageAssignmentsRepository _coverageAssignmentsRepo;

    public CoverageAssignmentsService(ICoverageAssignmentsRepository coverageAssignmentsRepo)
    {
        _coverageAssignmentsRepo = coverageAssignmentsRepo;
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

        await _coverageAssignmentsRepo.CreateAlertAsync(gapAlert);

        return Result.Ok("Assignment marked unavailable. Alert sent to supervisor.");
    }

}