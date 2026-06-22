using PCMS_Backend.DTOs;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Interfaces.Services;

public interface ICoverageScheduleService
{
    Task<Result<IReadOnlyList<CoverageScheduleDto>>> GetAllSchedulesAsync();

    Task<Result<CoverageScheduleDetailDto>> GetScheduleByIdAsync(int scheduleId);



    Task<Result<CoverageScheduleGenerateResponseDto>> GenerateScheduleAsync(int userId);

    Task<Result<bool>> PublishScheduleAsync(int scheduleId);


}