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
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICoverageAssignmentsRepository _coverageAssignmentsRepository;

    public SwapRequestService(
    ISwapRequestRepository repository,
    IPhysicianRepository physicianRepository,
    INotificationRepository notificationRepository,
    IUserRepository userRepository,
    ICoverageAssignmentsRepository coverageAssignmentsRepository
)
    {
        _repository = repository;
        _physicianRepository = physicianRepository;
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _coverageAssignmentsRepository = coverageAssignmentsRepository;
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
            var notification = new Notification
            {
                UserId = targetUserId.Value,
                NotificationTitle = "New Swap Request",
                NotificationMessage =
                    $"Dr. {physician.User.FullName} requested a swap for {targetAssignment.ShiftType} shift on {targetAssignment.CoverageDate:yyyy-MM-dd}.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.CreateAsync(notification);
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
            var notification = new Notification
            {
                UserId = requesterUserId.Value,
                NotificationTitle = "Swap Request Accepted",
                NotificationMessage =
                    $"Dr. {request.TargetPhysician.User.FullName} accepted your swap request. Waiting for supervisor approval.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.CreateAsync(notification);
        }

        var supervisor = await _userRepository.GetUserByIdWithRoleAsync(6);

        if (supervisor != null)
        {
            var supervisorNotification = new Notification
            {
                UserId = supervisor.UserId,
                NotificationTitle = "Swap Request Needs Approval",
                NotificationMessage =
                    $"Swap request between Dr. {request.RequestedByPhysician.User.FullName} and Dr. {request.TargetPhysician.User.FullName} is awaiting approval.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.CreateAsync(supervisorNotification);
        }

        return Result.Ok("Request approved");
    }


    public async Task<Result> ApproveRequestAsync(int swapRequestId, int userId)
    {
        var request = await _repository.GetByIdAsync(swapRequestId);

        if (request == null)
            return Result.NotFound("Request not found");



        var assignment = await _coverageAssignmentsRepository.GetAssignmentByIdAsync(request.CoverageAssignmentId);

        if (assignment == null)
            return Result.NotFound("Assignment not found");

        //    var targetPhysician = await _physicianRepository
        //.GetByIdAsync(request.TargetPhysicianId);

        //    if (targetPhysician == null)
        //        return Result.NotFound("Target physician not found");

        assignment.PhysicianId = request.TargetPhysicianId;

        await _coverageAssignmentsRepository.SaveChangesAsync();

        request.RequestStatus = "SUPERVISOR_APPROVED";
        request.ReviewedAt = DateTime.UtcNow;
        request.ReviewedByUserId = userId;


        await _repository.SaveChangesAsync();

        var requesterUserId = request.RequestedByPhysician.UserId;

        if (requesterUserId.HasValue)
        {
            var notification = new Notification
            {
                UserId = requesterUserId.Value,
                NotificationTitle = "Swap Request Approved By Supervisor",
                NotificationMessage =
                    $"Supervisor. {request.ReviewedByUser?.FullName} approved the Swap request .",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.CreateAsync(notification);
        }
        var targetUserId = request.TargetPhysicianId;




        var targetPhysicianNotification = new Notification
        {
            UserId = targetUserId,
            NotificationTitle = "Swap Request Approved By Supervisor",
            NotificationMessage =
                $"Supervisor. {request.ReviewedByUser?.FullName} approved the Swap request .",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

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
            var notification = new Notification
            {
                UserId = requesterUserId.Value,
                NotificationTitle = "Swap Request Declined",
                NotificationMessage =
                    $"Dr. {request.TargetPhysician.User.FullName} declined your swap request for {request.CoverageAssignment.ShiftType} shift on {request.CoverageAssignment.CoverageDate:yyyy-MM-dd}.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.CreateAsync(notification);
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
            var notification = new Notification
            {
                UserId = requesterUserId.Value,
                NotificationTitle = "Swap Request Rejected By Supervisor",
                NotificationMessage =
                     $"Supervisor. {request.ReviewedByUser?.FullName} rejected the Swap request .",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.CreateAsync(notification);
        }

        var targetUserId = request.TargetPhysicianId;




        var targetPhysicianNotification = new Notification
        {
            UserId = targetUserId,
            NotificationTitle = "Swap Request Approved By Supervisor",
            NotificationMessage =
                $"Supervisor. {request.ReviewedByUser?.FullName} declined the Swap request .",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.CreateAsync(targetPhysicianNotification);

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