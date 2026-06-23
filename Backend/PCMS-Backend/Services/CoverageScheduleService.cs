using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Services;

public class CoverageScheduleService : ICoverageScheduleService
{
    private readonly ICoverageScheduleRepository _coverageScheduleRepository;

    public CoverageScheduleService(
        ICoverageScheduleRepository coverageScheduleRepository)
    {
        _coverageScheduleRepository = coverageScheduleRepository;
    }

    public async Task<Result<IReadOnlyList<CoverageScheduleDto>>> GetAllSchedulesAsync()
    {
        var schedules = await _coverageScheduleRepository.GetAllAsync();

        var response = schedules
            .Select(cs => new CoverageScheduleDto
            {
                CoverageScheduleId = cs.CoverageScheduleId,
                ScheduleName = cs.ScheduleName,
                WeekStartDate = cs.WeekStartDate,
                WeekEndDate = cs.WeekEndDate,
                Status = cs.Status
            })
            .ToList();

        return Result<IReadOnlyList<CoverageScheduleDto>>
            .Ok(response);
    }

    public async Task<Result<CoverageScheduleDetailDto>> GetScheduleByIdAsync(int scheduleId)
    {
        var schedule = await _coverageScheduleRepository
            .GetByIdAsync(scheduleId);

        if (schedule is null)
        {
            return Result<CoverageScheduleDetailDto>
                .NotFound("Schedule not found.");
        }

        var response = new CoverageScheduleDetailDto
        {
            CoverageScheduleId = schedule.CoverageScheduleId,
            ScheduleName = schedule.ScheduleName,
            WeekStartDate = schedule.WeekStartDate,
            WeekEndDate = schedule.WeekEndDate,
            Status = schedule.Status,

            Assignments = schedule.CoverageAssignments
                .Select(ca => new CoverageAssignmentDto
                {
                    CoverageAssignmentId = ca.CoverageAssignmentId,
                    CoverageDate = ca.CoverageDate,
                    SpecialtyName = ca.Specialty.SpecialtyName,
                    PhysicianName = ca.Physician.User.FullName,
                    ShiftType = ca.ShiftType,
                    AssignmentStatus = ca.AssignmentStatus
                })
                .ToList()
        };

        return Result<CoverageScheduleDetailDto>
            .Ok(response);

    }
}