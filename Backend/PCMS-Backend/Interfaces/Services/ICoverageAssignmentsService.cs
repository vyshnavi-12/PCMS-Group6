using PCMS_Backend.DTOs;
using PCMS_Backend.Models;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Interfaces.Services;
public interface ICoverageAssignmentsService
{    
    Task<Result> MarkAssignmentUnavailableAsync(int assignmentId, string reason, int physicianId);
    Task<Result<IReadOnlyList<OpenAlertsResponseDto>>> GetAlertsAsync();
    Task<Result<AlertDetailsResponseDto>> GetAlertDetailsAsync(int alertId);
    Task<Result> UpdateAlertPhysicianAsync(int alertId, int physicianId);

    Task<Result> DeclineUnavailableRequestAsync(int alertId);
}