using PCMS_Backend.DTOs;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Interfaces.Services;

public interface IPhysicianService
{
    Task<Result<IReadOnlyList<GetAssignmentsDTO>>> GetAllAssignmentsAsync(int userId);
    Task<int?> GetPhysicianIdByUserIdAsync();
    Task<Result<IReadOnlyList<ReplacementPhysicianDto>>> GetSuggestedReplacementsAsync(int assignmentId);

    Task<Result<int>> GetUnavailableRequestCountAsync(int userId);
}