using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PCMS_Backend.DTOs;
using PCMS_Backend.Hubs;
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
    private readonly ICoverageAssignmentsRepository _coverageAssignmentsRepository;
    private readonly IHubContext<SwapRequestHub> _hubContext;
    private readonly INotificationRepository _notificationRepository;

    public SwapRequestService(
    ISwapRequestRepository repository,
    IPhysicianRepository physicianRepository,
    IUserRepository userRepository,
    INotificationService notificationService,
    IHubContext<SwapRequestHub> hubContext,
    INotificationRepository notificationRepository,
   
    ICoverageAssignmentsRepository coverageAssignmentsRepository
)
    {
        _repository = repository;
        _physicianRepository = physicianRepository;
        _userRepository = userRepository;
        _notificationService = notificationService;
        _hubContext = hubContext;
        _coverageAssignmentsRepository = coverageAssignmentsRepository;
        _notificationRepository = notificationRepository;
    }



    public async Task<Result<List<AvailableSwapTargetDto>>> GetAvailableTargetsAsync(int coverageAssignmentId)
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

    public async Task<Result> CreateSwapRequestAsync(int userId, CreateSwapRequestDto dto)
    {
        var physician = await _physicianRepository.GetByUserIdAsync(userId);
        if (physician == null)
            return Result.NotFound("Physician not found");

        var targetAssignment = await _repository.GetAssignmentByIdAsync(dto.TargetedPhysicianCoverageAssignmentId);
        if (targetAssignment == null)
            return Result.NotFound("Assignment not found");

        var request = new SwapRequest
        {
            RequestedPhysicianCoverageAssignmentId = dto.RequestedPhysicianCoverageAssignmentId,
            RequestedByPhysicianId = physician.PhysicianId,
            TargetedPhysicianCoverageAssignmentId=dto.TargetedPhysicianCoverageAssignmentId,
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
            // ✅ Send notification
            await _notificationService.CreateAndSendNotificationAsync(
                targetUserId.Value,
                "New Swap Request",
                $"Dr. {physician.User.FullName} requested a swap for {targetAssignment.ShiftType} shift on {targetAssignment.CoverageDate:yyyy-MM-dd}."
            );

            // ✅ Push real-time swap request via SignalR
            await _hubContext.Clients.Group($"User_{targetUserId.Value}")
                .SendAsync("ReceiveSwapRequest", new
                {
                    swapRequestId = request.SwapRequestId,
                    date = targetAssignment.CoverageDate.ToString("yyyy-MM-dd"),
                    shift = targetAssignment.ShiftType,
                    requestedBy = physician.User.FullName,
                    reason = request.RequestComments,
                    requestedOn = request.RequestedAt.ToString("yyyy-MM-dd HH:mm"),
                    status = request.RequestStatus
                });
        }

        return Result.Created("Swap request created");
    }

    public async Task<Result<List<MySwapRequestDto>>> GetMyRequestsAsync(int userId)
    {
        var physician = await _physicianRepository.GetByUserIdAsync(userId);

        if (physician == null)
            return Result<List<MySwapRequestDto>>.NotFound("Physician not found");

        var requests = await _repository.GetMyRequestsAsync(physician.PhysicianId);
        //changing MySwapRequestDto

        var response = requests.Select(r => new MySwapRequestDto
        {
            SwapRequestId = r.SwapRequestId,
            CurrentDate = r.RequestedPhysicianCoverageAssignment.CoverageDate.ToString("yyyy-MM-dd"),
            RequestedDate=r.TargetedPhysicianCoverageAssignment.CoverageDate.ToString("yyyy-MM-dd"),
            Shift = r.RequestedPhysicianCoverageAssignment.ShiftType,
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
            CurrentDate = r.TargetedPhysicianCoverageAssignment.CoverageDate.ToString("yyyy-MM-dd"),
            NewDate = r.RequestedPhysicianCoverageAssignment.CoverageDate.ToString("yyyy-MM-dd"),
            Shift = r.TargetedPhysicianCoverageAssignment.ShiftType,
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


    public async Task<Result> ApproveRequestAsync(int swapRequestId, int userId)
    {
        var swapRequest = await _repository.GetByIdAsync(swapRequestId);

        if (swapRequest == null)
            return Result.NotFound("Request not found");

        if (swapRequest.RequestedPhysicianCoverageAssignment==null || swapRequest.TargetedPhysicianCoverageAssignment==null)
            return Result.NotFound("Assignments not found");


        var temp = swapRequest.RequestedPhysicianCoverageAssignment.PhysicianId;

        swapRequest.RequestedPhysicianCoverageAssignment.PhysicianId =
           swapRequest.TargetedPhysicianCoverageAssignment.PhysicianId;

        swapRequest.TargetedPhysicianCoverageAssignment.PhysicianId = temp;

        //await _repository.SaveChangesAsync();







        //    var targetPhysician = await _physicianRepository
        //.GetByIdAsync(request.TargetPhysicianId);

        //    if (targetPhysician == null)
        //        return Result.NotFound("Target physician not found");

        //assignment.PhysicianId = request.TargetPhysicianId;

        //await _coverageAssignmentsRepository.SaveChangesAsync();

        swapRequest.RequestStatus = "SUPERVISOR_APPROVED";
        swapRequest.ReviewedAt = DateTime.UtcNow;
        swapRequest.ReviewedByUserId = userId;


        await _repository.SaveChangesAsync();

        var requesterUserId = swapRequest.RequestedByPhysician.UserId;

        if (requesterUserId.HasValue)
        {
            await _notificationService.CreateAndSendNotificationAsync(
                requesterUserId.Value,
                "Swap Request Approved By Supervisor",
                $"Supervisor approved the swap request."
            );
        }
        var targetUserId = swapRequest.TargetPhysicianId;




        var targetPhysicianNotification = new Notification
        {
            UserId = targetUserId,
            NotificationTitle = "Swap Request Approved By Supervisor",
            NotificationMessage =
                $"Supervisor. {swapRequest.ReviewedByUser?.FullName} approved the Swap request .",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
        

        await _notificationService.CreateAndSendNotificationAsync(
            targetUserId,
            "Swap Request Approved By Supervisor",
            $"Supervisor approved the swap request."
        );

        await _notificationRepository.CreateAsync(targetPhysicianNotification);


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
                $"Dr. {request.TargetPhysician.User.FullName} declined your swap request for {request.RequestedPhysicianCoverageAssignment.ShiftType} shift on {request.RequestedPhysicianCoverageAssignment.CoverageDate:yyyy-MM-dd}."
            );
        }

        return Result.Ok("Request declined");
    }

    public async Task<Result> RejectRequestAsync(int swapRequestId, int userId)
    {
        var request = await _repository.GetByIdAsync(swapRequestId);

        if (request == null)
            return Result.NotFound("Request not found");

        request.RequestStatus = "REQUEST_REJECTED";
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

        if (targetUserId.HasValue)
        {

            await _notificationService.CreateAndSendNotificationAsync(
                targetUserId.Value,
                "Swap Request Rejected By Supervisor",
                $"Supervisor rejected the swap request."
            );
        }

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
            Shift = r.RequestedPhysicianCoverageAssignment.ShiftType,
            RequestedPhysicianDate = r.RequestedPhysicianCoverageAssignment.CoverageDate.ToString("yyyy-MM-dd"),
            TargetedPhysicianDate = r.TargetedPhysicianCoverageAssignment.CoverageDate.ToString("yyyy-MM-dd"),
            Status = r.RequestStatus
        }).ToList();

        return Result<List<SupervisorSwapRequestDto>>.Ok(response);
    }
}