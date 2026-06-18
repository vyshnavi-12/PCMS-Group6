using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Repositories.Interfaces;
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
}