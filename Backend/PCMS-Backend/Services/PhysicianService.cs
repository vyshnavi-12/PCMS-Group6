using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Services;

public class PhysicianService : IPhysicianService
{
    private readonly IPhysicianRepository _physicianRepo;

    public PhysicianService(IPhysicianRepository physicianRepo)
    {
        _physicianRepo = physicianRepo;
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
}