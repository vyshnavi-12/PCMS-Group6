using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Services;

public class SwapRequestService : ISwapRequestService
{
    private readonly ISwapRequestRepository _repository;

    public SwapRequestService(
        ISwapRequestRepository repository
    )
    {
        _repository = repository;
    }

    public async Task<Result<List<AvailableSwapTargetDto>>> GetAvailableTargetsAsync(
        int coverageAssignmentId
    )
    {
        var selectedAssignment = await _repository
            .GetAssignmentByIdAsync(coverageAssignmentId);

        if (selectedAssignment == null)
        {
            return Result<List<AvailableSwapTargetDto>>
                .NotFound("Assignment not found");
        }

        var targets = await _repository.GetAvailableTargetsAsync(
              selectedAssignment.SpecialtyId,
              selectedAssignment.PhysicianId,
              selectedAssignment.ShiftType
         );

        var response = targets
            .Select(a => new AvailableSwapTargetDto
            {
                CoverageAssignmentId = a.CoverageAssignmentId,
                Date = a.CoverageDate.ToString("yyyy-MM-dd"),
                Shift = a.ShiftType,
                Specialty = a.Specialty.SpecialtyName,
                PhysicianName = a.Physician.User.FullName
            })
            .ToList();

        return Result<List<AvailableSwapTargetDto>>
            .Ok(response);
    }
}