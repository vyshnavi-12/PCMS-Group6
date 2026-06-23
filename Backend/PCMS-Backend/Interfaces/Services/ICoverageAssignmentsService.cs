using PCMS_Backend.Shared;
using PCMS_Backend.DTOs;

namespace PCMS_Backend.Interfaces.Services;
public interface ICoverageAssignmentsService
{    
    Task<Result> MarkAssignmentUnavailableAsync(int assignmentId, string reason, int physicianId);
    Task<Result<IReadOnlyList<OpenAlertsResponseDto>>> GetOpenAlertsAsync();
}