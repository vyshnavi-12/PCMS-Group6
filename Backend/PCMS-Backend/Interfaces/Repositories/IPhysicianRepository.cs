using PCMS_Backend.Models;

namespace PCMS_Backend.Interfaces.Repositories;

public interface IPhysicianRepository
{
    Task<Physician?> GetByCodeAsync(string physicianCode);
    void Update(Physician physician);

    Task<List<CoverageAssignment>> GetAssignmentsByUserIdAsync(int userId);
    Task<Physician?> GetByUserIdAsync(int userId);

    Task<int?> GetPhysicianIdByUserIdAsync(int userId);

    Task<int> GetUnavailableRequestsCountAsync(int physicianId);
}