using PCMS_Backend.Models;

namespace PCMS_Backend.Interfaces.Repositories;

public interface IPhysicianRepository
{
    Task<Physician?> GetByCodeAsync(string physicianCode);
    void Update(Physician physician);
}