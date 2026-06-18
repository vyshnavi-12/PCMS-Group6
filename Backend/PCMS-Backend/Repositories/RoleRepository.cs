using PCMS_Backend.Data;
using PCMS_Backend.Interfaces.Repositories; 
using PCMS_Backend.Models;

namespace PCMS_Backend.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly PcmsDbContext _context;

    public RoleRepository(PcmsDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(int roleId)
    {
        return await _context.Roles.FindAsync(roleId);
    }
}