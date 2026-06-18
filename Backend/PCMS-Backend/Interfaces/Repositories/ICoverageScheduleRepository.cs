using PCMS_Backend.Models;

namespace PCMS_Backend.Interfaces.Repositories;

public interface ICoverageScheduleRepository
{
    Task<IReadOnlyList<CoverageSchedule>> GetAllAsync();
}