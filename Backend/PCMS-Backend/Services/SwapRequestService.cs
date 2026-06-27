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
    }

    private async Task<bool> IsConflictAsync(int physicianId, CoverageAssignment assignment)
    {
        var date = assignment.CoverageDate;
        var shift = assignment.ShiftType;
        if (await _physicianRepository
       .IsPhysicianOnLeaveAsync(physicianId, assignment.CoverageDate))
        {
            return true;
        }



        var physicianActiveAssignments =
            await _coverageAssignmentsRepository.GetActiveAssignmentsByPhysicianIdAsync(physicianId);

        foreach (var existingAssignment in physicianActiveAssignments)
        {
            // Already assigned on the same date
            if (existingAssignment.Date == date)
            {
                return true;
            }

            // Previous night's shift prevents today's day shift
            if (shift == "Day" &&
                existingAssignment.Date == date.AddDays(-1) &&
                existingAssignment.ShiftType == "Night")
            {
                return true;
            }

            // Today's night shift prevents next day's day shift
            if (shift == "Night" &&
                existingAssignment.Date == date.AddDays(1) &&
                existingAssignment.ShiftType == "Day")
            {
                return true;
            }
        }

        return false;
    }



    public async Task<Result<List<AvailableSwapTargetDto>>> GetAvailableTargetsAsync(int coverageAssignmentId)
    {
        var selectedAssignment = await _repository.GetAssignmentByIdAsync(coverageAssignmentId);
        if (selectedAssignment == null)
            return Result<List<AvailableSwapTargetDto>>.NotFound("Assignment not found");

        var targets = await _repository.GetAvailableTargetsAsync(
            selectedAssignment.SpecialtyId,
            selectedAssignment.PhysicianId,
            selectedAssignment.ShiftType,
            selectedAssignment.CoverageScheduleId
        );

        var validTargets = new List<CoverageAssignment>();

        foreach (var target in targets)
        {
            var requesterConflict = await _repository.HasOtherAssignmentOnDateAsync(
                selectedAssignment.PhysicianId,
                target.CoverageDate,
                selectedAssignment.CoverageAssignmentId
            );

            var targetConflict = await _repository.HasAssignmentOnDateAsync(
                target.PhysicianId,
                selectedAssignment.CoverageDate
            );

            if (!requesterConflict && !targetConflict)
            {
                validTargets.Add(target);
            }
        }

        var response = validTargets.Select(a => new AvailableSwapTargetDto
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
        var targetphysician = await _physicianRepository.GetByUserIdAsync(dto.TargetPhysicianId);
        if (targetphysician == null)
            return Result.NotFound("Target Physician not found");
        var currentAssignment= await _repository.GetAssignmentByIdAsync(dto.RequestedPhysicianCoverageAssignmentId);


        if(currentAssignment == null)
            return Result.NotFound(" Current Assignment not found");


        var targetAssignment = await _repository.GetAssignmentByIdAsync(dto.TargetedPhysicianCoverageAssignmentId);

        if(targetAssignment == null)
            return Result.NotFound(" Target Assignment not found");
        DateOnly currdate = DateOnly.FromDateTime(DateTime.Today);
        if (currdate >= currentAssignment.CoverageDate)
            return Result.Forbidden("can not swap tadays or previos day assignment");
        if (currdate >= targetAssignment.CoverageDate)
            return Result.Forbidden("can not swap with todays or previous days assignment");
        if (currentAssignment.SpecialtyId != targetAssignment.SpecialtyId)
            return Result.Forbidden("assignments specialities mismatch");
        if (currentAssignment.PhysicianId != physician.PhysicianId)
            return Result.Forbidden("Given current coverage Assignment does not belong to requested Physician");

        if (targetAssignment.PhysicianId != dto.TargetPhysicianId)
            return Result.Forbidden("Targeted cover Assignment does not belong to Targeted Physician");

        if (currentAssignment.CoverageScheduleId != targetAssignment.CoverageScheduleId)
            return Result.Forbidden("coverage Schedules mismatch");

        var isconflictingRequested = await IsConflictAsync(physician.PhysicianId, targetAssignment);
        if (isconflictingRequested)
            return Result.Forbidden("Requested physicians schedule conflicts with target assignment");

        var isconflictingTargeted=await IsConflictAsync(targetphysician.PhysicianId, currentAssignment);

        if(isconflictingTargeted)
            return Result.Forbidden("Targeted physicians schedule conflicts with current assignment");








        if (targetAssignment.CoverageDate == DateOnly.FromDateTime(DateTime.Today))
        {
            return Result.BadRequest("Swap request is not allowed for current date.");
        }

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
            await _notificationService.CreateAndSendNotificationAsync(
                targetUserId.Value,
                "New Swap Request",
                $"Dr. {physician.User.FullName} requested a swap for {targetAssignment.ShiftType} shift on {targetAssignment.CoverageDate:yyyy-MM-dd}."
            );

            await _hubContext.Clients.Group($"User_{targetUserId.Value}")
               .SendAsync("RefreshSwapRequests");

            await _hubContext.Clients.Group($"User_{userId}")
               .SendAsync("RefreshSwapRequests");
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

        await _hubContext.Clients.Group($"User_{request.RequestedByPhysician.UserId}")
             .SendAsync("RefreshSwapRequests");

        await _hubContext.Clients.Group($"User_{request.TargetPhysician.UserId}")
            .SendAsync("RefreshSwapRequests");

        await _hubContext.Clients.Group("User_6")
            .SendAsync("RefreshSupervisorSwapRequests");

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

        var targetUserId = swapRequest.TargetPhysician.UserId;

        await _notificationService.CreateAndSendNotificationAsync(
            targetUserId.Value,
            "Swap Request Approved By Supervisor",
            $"Supervisor approved the swap request."
        );

        await _hubContext.Clients.Group($"User_{swapRequest.RequestedByPhysician.UserId}")
            .SendAsync("RefreshSwapRequests");

        await _hubContext.Clients.Group($"User_{swapRequest.TargetPhysician.UserId}")
            .SendAsync("RefreshSwapRequests");

        await _hubContext.Clients.Group("User_6")
            .SendAsync("RefreshSupervisorSwapRequests");


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

        await _hubContext.Clients.Group($"User_{request.RequestedByPhysician.UserId}")
            .SendAsync("RefreshSwapRequests");

        await _hubContext.Clients.Group($"User_{request.TargetPhysician.UserId}")
            .SendAsync("RefreshSwapRequests");

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

        await _hubContext.Clients.Group($"User_{request.RequestedByPhysician.UserId}")
            .SendAsync("RefreshSwapRequests");

        await _hubContext.Clients.Group($"User_{request.TargetPhysician.UserId}")
            .SendAsync("RefreshSwapRequests");

        await _hubContext.Clients.Group("User_6")
            .SendAsync("RefreshSupervisorSwapRequests");

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

    public async Task<Result<int>> GetTargetAcceptedCountAsync()
    {
        var count = await _repository.GetTargetAcceptedCountAsync();
        return Result<int>.Ok(count);
    }
    public async Task<Result<int>> GetPendingMyRequestsCountAsync(int userId)
    {
        var physician = await _physicianRepository.GetByUserIdAsync(userId);
        if (physician == null)
            return Result<int>.NotFound("Physician not found");

        var count = await _repository.GetPendingMyRequestsCountAsync(physician.PhysicianId);
        return Result<int>.Ok(count);
    }

    public async Task<Result<int>> GetPendingRequestsToMeCountAsync(int userId)
    {
        var physician = await _physicianRepository.GetByUserIdAsync(userId);
        if (physician == null)
            return Result<int>.NotFound("Physician not found");

        var count = await _repository.GetPendingRequestsToMeCountAsync(physician.PhysicianId);
        return Result<int>.Ok(count);
    }

}