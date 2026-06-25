using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Models;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Services;

public class SwapRequestService : ISwapRequestService
{
    private readonly ISwapRequestRepository _repository;
    private readonly IPhysicianRepository _physicianRepository;
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;

    public SwapRequestService(
    ISwapRequestRepository repository,
    IPhysicianRepository physicianRepository,
    IUserRepository userRepository,
    INotificationService notificationService
)
    {
        _repository = repository;
        _physicianRepository = physicianRepository;
        _userRepository = userRepository;
        _notificationService = notificationService;
    }

    public async Task<Result<List<AvailableSwapTargetDto>>> GetAvailableTargetsAsync(
        int coverageAssignmentId
    )
    {
        var selectedAssignment = await _repository.GetAssignmentByIdAsync(coverageAssignmentId);

        if (selectedAssignment == null)
            return Result<List<AvailableSwapTargetDto>>.NotFound("Assignment not found");

        var targets = await _repository.GetAvailableTargetsAsync(
            selectedAssignment.SpecialtyId,
            selectedAssignment.PhysicianId,
            selectedAssignment.ShiftType
        );

        var response = targets.Select(a => new AvailableSwapTargetDto
        {
            CoverageAssignmentId = a.CoverageAssignmentId,
            Date = a.CoverageDate.ToString("yyyy-MM-dd"),
            Shift = a.ShiftType,
            Specialty = a.Specialty.SpecialtyName,
            PhysicianName = a.Physician.User.FullName,
            PhysicianId = a.PhysicianId
        }).ToList();

        return Result<List<AvailableSwapTargetDto>>.Ok(response);
    }

    public async Task<Result> CreateSwapRequestAsync(
        int userId,
        CreateSwapRequestDto dto
    )
    {
        var physician = await _physicianRepository.GetByUserIdAsync(userId);

        if (physician == null)
            return Result.NotFound("Physician not found");

        var targetAssignment = await _repository.GetAssignmentByIdAsync(dto.CoverageAssignmentId);

        if (targetAssignment == null)
            return Result.NotFound("Assignment not found");

        var request = new SwapRequest
        {
            CoverageAssignmentId = dto.CoverageAssignmentId,
            RequestedByPhysicianId = physician.PhysicianId,
            TargetPhysicianId = dto.TargetPhysicianId,
            RequestComments = dto.RequestComments,
            RequestStatus = "PENDING_TARGET",
            RequestedAt = DateTime.UtcNow
        };

        await _repository.CreateAsync(request);
        await _repository.SaveChangesAsync();

        var targetUserId = targetAssignment.Physician.UserId;

        if (targetUserId.HasValue)
        {
            await _notificationService.CreateAndSendNotificationAsync(
                targetUserId.Value,
                "New Swap Request",
                $"Dr. {physician.User.FullName} requested a swap for {targetAssignment.ShiftType} shift on {targetAssignment.CoverageDate:yyyy-MM-dd}."
            );
        }

        return Result.Created("Swap request created");
    }

    public async Task<Result<List<MySwapRequestDto>>> GetMyRequestsAsync(int userId)
    {
        var physician = await _physicianRepository.GetByUserIdAsync(userId);

        if (physician == null)
            return Result<List<MySwapRequestDto>>.NotFound("Physician not found");

        var requests = await _repository.GetMyRequestsAsync(physician.PhysicianId);

        var response = requests.Select(r => new MySwapRequestDto
        {
            SwapRequestId = r.SwapRequestId,
            Date = r.CoverageAssignment.CoverageDate.ToString("yyyy-MM-dd"),
            Shift = r.CoverageAssignment.ShiftType,
            RequestedWith = r.TargetPhysician.User.FullName,
            Status = r.RequestStatus,
            RequestedOn = r.RequestedAt.ToString("yyyy-MM-dd HH:mm"),
            Reason = r.RequestComments
        }).ToList();

        return Result<List<MySwapRequestDto>>.Ok(response);
    }

    public async Task<Result<List<RequestToMeDto>>> GetRequestsToMeAsync(int userId)
    {
        var physician = await _physicianRepository.GetByUserIdAsync(userId);

        if (physician == null)
            return Result<List<RequestToMeDto>>.NotFound("Physician not found");

        var requests = await _repository.GetRequestsToMeAsync(physician.PhysicianId);

        var response = requests.Select(r => new RequestToMeDto
        {
            SwapRequestId = r.SwapRequestId,
            Date = r.CoverageAssignment.CoverageDate.ToString("yyyy-MM-dd"),
            Shift = r.CoverageAssignment.ShiftType,
            RequestedBy = r.RequestedByPhysician.User.FullName,
            Reason = r.RequestComments,
            RequestedOn = r.RequestedAt.ToString("yyyy-MM-dd HH:mm"),
            Status = r.RequestStatus
        }).ToList();

        return Result<List<RequestToMeDto>>.Ok(response);
    }

    public async Task<Result> AcceptRequestAsync(int swapRequestId, int userId)
    {
        var request = await _repository.GetByIdAsync(swapRequestId);

        if (request == null)
            return Result.NotFound("Request not found");

        request.RequestStatus = "TARGET_ACCEPTED";
        request.RespondedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();

        var requesterUserId = request.RequestedByPhysician.UserId;

        if (requesterUserId.HasValue)
        {
            await _notificationService.CreateAndSendNotificationAsync(
                requesterUserId.Value,
                "Swap Request Accepted",
                $"Dr. {request.TargetPhysician.User.FullName} accepted your swap request. Waiting for supervisor approval."
            );
        }

        var supervisor = await _userRepository.GetUserByIdWithRoleAsync(6);

        if (supervisor != null)
        {
            await _notificationService.CreateAndSendNotificationAsync(
                supervisor.UserId,
                "Swap Request Needs Approval",
                $"Swap request between Dr. {request.RequestedByPhysician.User.FullName} and Dr. {request.TargetPhysician.User.FullName} is awaiting approval."
            );
        }

        return Result.Ok("Request approved");
    }


    public async Task<Result> ApproveRequestAsync(int swapRequestId,int userId)
    {
        var request = await _repository.GetByIdAsync(swapRequestId);

        if (request == null)
            return Result.NotFound("Request not found");

        request.RequestStatus = "SUPERVISOR_APPROVED";
        request.ReviewedAt = DateTime.UtcNow;
        request.ReviewedByUserId = userId;


        await _repository.SaveChangesAsync();

        var requesterUserId = request.RequestedByPhysician.UserId;

        if (requesterUserId.HasValue)
        {
            await _notificationService.CreateAndSendNotificationAsync(
                requesterUserId.Value,
                "Swap Request Approved By Supervisor",
                $"Supervisor approved the swap request."
            );
        }
        var targetUserId = request.TargetPhysician.UserId;

        await _notificationService.CreateAndSendNotificationAsync(
            targetUserId.Value,
            "Swap Request Approved By Supervisor",
            $"Supervisor approved the swap request."
        );


        return Result.Ok("Request approved");
    }

    public async Task<Result> DeclineRequestAsync(int swapRequestId, int userId)
    {
        var request = await _repository.GetByIdAsync(swapRequestId);

        if (request == null)
            return Result.NotFound("Request not found");

        request.RequestStatus = "TARGET_DECLINED";
        request.RespondedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();

        var requesterUserId = request.RequestedByPhysician.UserId;

        if (requesterUserId.HasValue)
        {
            await _notificationService.CreateAndSendNotificationAsync(
                requesterUserId.Value,
                "Swap Request Declined",
                $"Dr. {request.TargetPhysician.User.FullName} declined your swap request for {request.CoverageAssignment.ShiftType} shift on {request.CoverageAssignment.CoverageDate:yyyy-MM-dd}."
            );
        }

        return Result.Ok("Request declined");
    }

    public async Task<Result> RejectRequestAsync(int swapRequestId, int userId)
    {
        var request = await _repository.GetByIdAsync(swapRequestId);

        if (request == null)
            return Result.NotFound("Request not found");

        request.RequestStatus = "SUPERVISOR_DECLINED";
        request.ReviewedAt = DateTime.UtcNow;
        request.ReviewedByUserId = userId;

        await _repository.SaveChangesAsync();

        var requesterUserId = request.RequestedByPhysician.UserId;

        if (requesterUserId.HasValue)
        {
            await _notificationService.CreateAndSendNotificationAsync(
                requesterUserId.Value,
                "Swap Request Rejected By Supervisor",
                $"Supervisor rejected the swap request."
            );
        }

        var targetUserId = request.TargetPhysician.UserId;

        await _notificationService.CreateAndSendNotificationAsync(
            targetUserId.Value,
            "Swap Request Rejected By Supervisor",
            $"Supervisor rejected the swap request."
        );

        return Result.Ok("Request Rejected");
    }

    public async Task<Result<List<SupervisorSwapRequestDto>>> GetSupervisorRequestsAsync()
    {
        var requests = await _repository.GetSupervisorRequestsAsync();

        var response = requests.Select(r => new SupervisorSwapRequestDto
        {
            SwapRequestId = r.SwapRequestId,
            RequestedBy = r.RequestedByPhysician.User.FullName,
            TargetPhysician = r.TargetPhysician.User.FullName,
            Shift = r.CoverageAssignment.ShiftType,
            Date = r.CoverageAssignment.CoverageDate.ToString("yyyy-MM-dd"),
            Status = r.RequestStatus
        }).ToList();

        return Result<List<SupervisorSwapRequestDto>>.Ok(response);
    }
}