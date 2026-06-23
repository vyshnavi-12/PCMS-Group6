using PCMS_Backend.Models;

namespace PCMS_Backend.Interfaces.Repositories;

public interface ISupervisorRepository
{
    Task<List<CoverageGapAlert>> GetOpenGapAlertsAsync();
}