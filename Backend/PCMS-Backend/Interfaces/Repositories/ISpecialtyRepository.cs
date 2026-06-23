using PCMS_Backend.Models;

namespace PCMS_Backend.Interfaces.Repositories;

public interface ISpecialtyRepository
{
    Task<IReadOnlyList<Specialty>> GetAllAsync();
}