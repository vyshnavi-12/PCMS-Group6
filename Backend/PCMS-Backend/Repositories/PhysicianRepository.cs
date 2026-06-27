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

    public async Task<List<CoverageAssignment>> GetAssignmentsByUserIdAsync(int userId)
    {
        return await _context.CoverageAssignments
            .Include(ca => ca.Physician)
            .Include(ca => ca.Specialty)
            .Include(ca => ca.CoverageSchedule)
            .Where(ca => ca.Physician.UserId == userId)
            .OrderBy(ca => ca.CoverageDate)
            .ToListAsync();
    }

    public void Update(Physician physician)
    {
        _context.Physicians.Update(physician);
    }

    public async Task<Physician?> GetByUserIdAsync(int userId)
    {
        return await _context.Physicians
            .Include(p => p.User)
            .Include(p => p.PhysicianSpecialtyMaps)
                .ThenInclude(ps => ps.Specialty)
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<int?> GetPhysicianIdByUserIdAsync(int userId)
    {
        return await _context.Physicians
            .Where(p => p.UserId == userId)
            .Select(p => p.PhysicianId)
            .FirstOrDefaultAsync();
    }
    public async Task<bool> IsPhysicianOnLeaveAsync(int physicianId, DateOnly date)
    {
        return await _context.ExternalLeavesData
            .AnyAsync(l =>
                l.PhysicianId == physicianId &&
                l.LeaveStartDate <= date &&
                l.LeaveEndDate >= date);
    }

    public async Task<int> GetUnavailableRequestsCountAsync(int physicianId)
    {
        return await _context.CoverageGapAlerts.CountAsync(cga => cga.RequestedByPhysicianId == physicianId);
    }

    public async Task<int> GetUnavailableRequestsCountSupervisorAsync()
    {
        return await _context.CoverageGapAlerts.CountAsync(cga => cga.AlertStatus == "Open");
    }
}