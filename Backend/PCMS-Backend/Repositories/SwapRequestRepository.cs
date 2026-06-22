using Microsoft.EntityFrameworkCore;
using PCMS_Backend.Data;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Models;

namespace PCMS_Backend.Repositories;

public class SwapRequestRepository : ISwapRequestRepository
{
    private readonly PcmsDbContext _context;

    public SwapRequestRepository(PcmsDbContext context)
    {
        _context = context;
    }

    public async Task<CoverageAssignment?> GetAssignmentByIdAsync(
        int coverageAssignmentId
    )
    {
        return await _context.CoverageAssignments
            .Include(a => a.Specialty)
            .Include(a => a.Physician)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(a =>
                a.CoverageAssignmentId == coverageAssignmentId);
    }

    public async Task<List<CoverageAssignment>> GetAvailableTargetsAsync(
    int specialtyId,
    int excludedPhysicianId,
    string shiftType
)
    {
        return await _context.CoverageAssignments
            .Include(a => a.Specialty)
            .Include(a => a.Physician)
                .ThenInclude(p => p.User)
            .Where(a =>
                a.SpecialtyId == specialtyId &&
                a.PhysicianId != excludedPhysicianId &&
                a.ShiftType == shiftType)
            .ToListAsync();
    }
}