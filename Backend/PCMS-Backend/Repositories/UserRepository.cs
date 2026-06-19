using Microsoft.EntityFrameworkCore;
using PCMS_Backend.Data;
using PCMS_Backend.Interfaces.Repositories; // <-- Updated
using PCMS_Backend.Models;

namespace PCMS_Backend.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PcmsDbContext _context;

    public UserRepository(PcmsDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByEmployeeCodeAsync(string employeeCode)
    {
        return await _context.Users.AnyAsync(u => u.EmployeeCode == employeeCode);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.EmailAddress == email);
    }

    public async Task<User?> GetUserByEmailWithRoleAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.EmailAddress == email);
    }

    public async Task<User?> GetUserByIdWithRoleAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Physician)
                .ThenInclude(p => p.PhysicianSpecialtyMaps)
                    .ThenInclude(psm => psm.Specialty)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}