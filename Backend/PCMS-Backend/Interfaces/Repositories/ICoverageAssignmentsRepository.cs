using PCMS_Backend.DTOs;
using PCMS_Backend.Models;
using PCMS_Backend.Shared;
namespace PCMS_Backend.Interfaces.Repositories;

public interface ICoverageAssignmentsRepository
{
    Task<CoverageAssignment?> GetAssignmentByIdAsync(int assignmentId);
    Task CreateAlertAsync(CoverageGapAlertDto alert, int physicianId);

    Task<IReadOnlyList<OpenAlertsResponseDto>> GetAlertsAsync();

    Task<CoverageGapAlert?> GetAlertDetailsByIdAsync(int alertId);
    Task<bool> UpdateAssignmentPhysicianAsync(int alertId, int physicianId);

    Task<bool> UpdateAlertStatusToResolvedAsync(int alertId);

    Task<IReadOnlyList<UnavailableRequestsPerSpecialtyDto>> GetUnavailableRequestsPerSpecialtyAsync();
    Task<List<AssignmentInfoDto>> GetActiveAssignmentsByPhysicianIdAsync(int physicianId);

    Task SaveChangesAsync();


}