using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Services;

public class SupervisorService : ISupervisorService
{
    private readonly ISupervisorRepository _supervisorRepo;

    public SupervisorService(ISupervisorRepository supervisorRepo)
    {
        _supervisorRepo = supervisorRepo;
    }

    public async Task<Result<List<OpenGapAlertDto>>> GetOpenGapAlertsAsync()
    {
        var alerts = await _supervisorRepo.GetOpenGapAlertsAsync();

        if (alerts == null || !alerts.Any())
        {
            return Result<List<OpenGapAlertDto>>.Ok(
                new List<OpenGapAlertDto>(),
                "No open coverage gaps found."
            );
        }

        var dtoList = alerts.Select(a => new OpenGapAlertDto
        {
            CoverageGapAlertId = a.CoverageGapAlertId,
            CoverageAssignmentId = a.CoverageAssignmentId,
            AlertStatus = a.AlertStatus,
            AlertReason = a.AlertReason,
            CreatedAt = a.CreatedAt,

            // Safe navigation operators in case of bad DB constraints
            CoverageDate = a.CoverageAssignment?.CoverageDate ?? default,
            ShiftType = a.CoverageAssignment?.ShiftType ?? "Unknown",
            SpecialtyName = a.CoverageAssignment?.Specialty?.SpecialtyName ?? "Unknown"
        }).ToList();

        return Result<List<OpenGapAlertDto>>.Ok(dtoList, "Open gap alerts retrieved successfully.");
    }
}