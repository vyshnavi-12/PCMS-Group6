using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Services;

public class PhysicianService : IPhysicianService
{
    private readonly IPhysicianRepository _physicianRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PhysicianService(IPhysicianRepository physicianRepo, IHttpContextAccessor httpContextAccessor)
    {
        _physicianRepo = physicianRepo;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<IReadOnlyList<GetAssignmentsDTO>>> GetAllAssignmentsAsync(int userId)
    {
        var assignments = await _physicianRepo.GetAssignmentsByUserIdAsync(userId);

        if (assignments == null || !assignments.Any())
        {
            return Result<IReadOnlyList<GetAssignmentsDTO>>.Ok(
                new List<GetAssignmentsDTO>(),
                "No assignments found."
            );
        }

        var dtoList = assignments.Select(a => new GetAssignmentsDTO
        {
            CoverageAssignmentId = a.CoverageAssignmentId,
            CoverageScheduleId = a.CoverageScheduleId ,
            ScheduleName = a.CoverageSchedule?.ScheduleName ?? "Unpublished",
            CoverageDate = a.CoverageDate,
            SpecialtyId = a.SpecialtyId,
            SpecialtyName = a.Specialty?.SpecialtyName ?? "Unknown",
            PhysicianId = a.PhysicianId,
            ShiftType = a.ShiftType,
            AssignmentStatus = a.AssignmentStatus,
            CreatedAt = a.CreatedAt 
        }).ToList();

        return Result<IReadOnlyList<GetAssignmentsDTO>>.Ok(dtoList, "Assignments retrieved successfully.");
    }
    public async Task<int?> GetPhysicianIdByUserIdAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User.GetCurrentUserId();
        if (userId is not int validUserId) return null;
        var physicianId = await _physicianRepo.GetPhysicianIdByUserIdAsync(validUserId);
        if (physicianId is not null) return physicianId;
        return null;
    }
}