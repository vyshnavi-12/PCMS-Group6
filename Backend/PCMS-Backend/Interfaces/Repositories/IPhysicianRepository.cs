using PCMS_Backend.Models;

namespace PCMS_Backend.Interfaces.Repositories;

public interface IPhysicianRepository
{
    Task<Physician?> GetByCodeAsync(string physicianCode);
    void Update(Physician physician);
<<<<<<< HEAD

    Task<List<CoverageAssignment>> GetAssignmentsByUserIdAsync(int userId);
=======
    Task<Physician?> GetByUserIdAsync(int userId);
>>>>>>> 953e3e9f50b6e5ce935603dcab37698b53d7f3ad
}