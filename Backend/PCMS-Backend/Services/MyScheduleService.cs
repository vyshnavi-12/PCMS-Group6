using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Services;

public class MyScheduleService : IMyScheduleService
{
    private readonly IMyScheduleRepository _repository;
    private readonly IPhysicianRepository _physicianRepository;

    public MyScheduleService(
        IMyScheduleRepository repository,
        IPhysicianRepository physicianRepository)
    {
        _repository = repository;
        _physicianRepository = physicianRepository;
    }

    public async Task<Result<IReadOnlyList<DoctorScheduleDto>>> GetMyScheduleAsync(int userId)
    {
        var physician = await _physicianRepository.GetByUserIdAsync(userId);

        if (physician is null)
        {
            return Result<IReadOnlyList<DoctorScheduleDto>>
                .NotFound("Physician not found.");
        }

        var assignments = await _repository
            .GetPhysicianAssignmentsAsync(physician.PhysicianId);

        var response = assignments.Select(a => new DoctorScheduleDto
        {
            Date = a.CoverageDate,
            Shift = a.ShiftType,
            Specialty = a.Specialty.SpecialtyName,
            Time = a.ShiftType == "Day"
                ? "08:00 AM - 04:00 PM"
                : "04:00 PM - 12:00 AM",
            Status = "ASSIGNED"
        }).ToList();

        return Result<IReadOnlyList<DoctorScheduleDto>>
            .Ok(response);
    }
}