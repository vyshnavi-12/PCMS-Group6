using PCMS_Backend.DTOs;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Interfaces.Services;

public interface ISwapRequestService
{
    Task<Result<List<AvailableSwapTargetDto>>> GetAvailableTargetsAsync(
        int coverageAssignmentId
    );

    Task<Result> CreateSwapRequestAsync(
    int userId,
    CreateSwapRequestDto dto
    );

    Task<Result<List<MySwapRequestDto>>> GetMyRequestsAsync(int userId);

    Task<Result<List<RequestToMeDto>>> GetRequestsToMeAsync(int userId);

    Task<Result> ApproveRequestAsync(int swapRequestId, int userId);
    Task<Result> AcceptRequestAsync(int swapRequestId, int userId);


    Task<Result> DeclineRequestAsync(int swapRequestId, int userId);
    Task<Result> RejectRequestAsync(int swapRequestId, int userId);


    Task<Result<List<SupervisorSwapRequestDto>>> GetSupervisorRequestsAsync();
    Task<Result<int>> GetTargetAcceptedCountAsync();
}