using Microsoft.EntityFrameworkCore;
using PCMS_Backend.Data;
using PCMS_Backend.Interfaces.Repositories; 
using PCMS_Backend.Models;

namespace PCMS_Backend.Repositories;

public class PhysicianRepository : IPhysicianRepository
{
    private readonly PcmsDbContext _context;

    public PhysicianRepository(PcmsDbContext context)
    {
        _context = context;
    }

    public async Task<Physician?> GetByCodeAsync(string physicianCode)
    {
        return await _context.Physicians
            .FirstOrDefaultAsync(p => p.PhysicianCode == physicianCode);
    }

    public void Update(Physician physician)
    {
        _context.Physicians.Update(physician);
    }

    public async Task<Physician?> GetByUserIdAsync(int userId)
    {
        return await _context.Physicians
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }
}