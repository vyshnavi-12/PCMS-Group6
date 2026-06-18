using PCMS_Backend.Models;

namespace PCMS_Backend.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(int roleId);
}