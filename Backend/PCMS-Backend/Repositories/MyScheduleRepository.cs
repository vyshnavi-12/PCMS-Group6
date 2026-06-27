using Microsoft.EntityFrameworkCore;
using PCMS_Backend.Data;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Models;

namespace PCMS_Backend.Repositories;

public class MyScheduleRepository : IMyScheduleRepository
{
    private readonly PcmsDbContext _context;

    public MyScheduleRepository(PcmsDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CoverageAssignment>> GetPhysicianAssignmentsAsync(int physicianId)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        return await _context.CoverageAssignments
            .AsNoTracking()
            .Include(c => c.Specialty)
            .Include(c => c.CoverageSchedule)
            .Where(c =>
                c.PhysicianId == physicianId &&
                c.CoverageSchedule.Status == "PUBLISHED" &&
                c.CoverageDate >= today)
            .OrderBy(c => c.CoverageDate)
            .ToListAsync();
    }
}