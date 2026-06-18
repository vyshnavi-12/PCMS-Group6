using PCMS_Backend.Models;

namespace PCMS_Backend.Repositories.Interfaces;

public interface ICoverageScheduleRepository
{
    Task<IReadOnlyList<CoverageSchedule>> GetAllAsync();
}