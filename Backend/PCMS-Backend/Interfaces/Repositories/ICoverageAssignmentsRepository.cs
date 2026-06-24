using PCMS_Backend.DTOs;
using PCMS_Backend.Models;
namespace PCMS_Backend.Interfaces.Repositories;

public interface ICoverageAssignmentsRepository
{
    Task<CoverageAssignment?> GetAssignmentByIdAsync(int assignmentId);
    Task CreateAlertAsync(CoverageGapAlertDto alert);

    Task<IReadOnlyList<OpenAlertsResponseDto>> GetOpenAlertsAsync();
    Task<List<TopPhysicianRawDto>> GetTopPhysiciansPerSpecialtyRawAsync(int scheduleId);
    Task<List<AssignmentRawDto>> GetAssignmentsByPhysiciansAsync(int scheduleId, List<int> physicianIds);
}