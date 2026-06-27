using PCMS_Backend.Models;

namespace PCMS_Backend.Interfaces.Repositories;

public interface ISwapRequestRepository
{
    Task<CoverageAssignment?> GetAssignmentByIdAsync(int coverageAssignmentId);

    Task<List<CoverageAssignment>> GetAvailableTargetsAsync(
        int specialtyId,
        int excludedPhysicianId,
        string shiftType,
        int coverageScheduleId
    );

    Task<bool> HasAssignmentOnDateAsync(
        int physicianId,
        DateOnly date
    );

    Task<bool> HasOtherAssignmentOnDateAsync(
        int physicianId,
        DateOnly date,
        int excludedAssignmentId
    );

    Task CreateAsync(SwapRequest request);

    Task<List<SwapRequest>> GetMyRequestsAsync(int physicianId);

    Task<List<SwapRequest>> GetRequestsToMeAsync(int physicianId);
    Task<int> GetPendingApprovalSwapRequestCount();

    Task<SwapRequest?> GetByIdAsync(int swapRequestId);

    Task<List<SwapRequest>> GetSupervisorRequestsAsync();

    Task SaveChangesAsync();
    Task<int> GetTargetAcceptedCountAsync();

    Task<int> GetPendingMyRequestsCountAsync(int physicianId);
    Task<int> GetPendingRequestsToMeCountAsync(int physicianId);
}