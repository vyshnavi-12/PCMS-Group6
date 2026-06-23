using PCMS_Backend.DTOs;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Interfaces.Services;

public interface ISwapRequestService
{
    Task<Result<List<AvailableSwapTargetDto>>> GetAvailableTargetsAsync(
        int coverageAssignmentId
    );
}