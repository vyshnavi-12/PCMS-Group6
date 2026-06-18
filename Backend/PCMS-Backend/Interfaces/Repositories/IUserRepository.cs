using PCMS_Backend.Models;

namespace PCMS_Backend.Interfaces.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsByEmployeeCodeAsync(string employeeCode);
    Task<bool> ExistsByEmailAsync(string email);
    Task<User?> GetUserByEmailWithRoleAsync(string email);
    Task<User?> GetUserByIdWithRoleAsync(int userId);
    Task AddAsync(User user);
    Task<int> SaveChangesAsync();
}