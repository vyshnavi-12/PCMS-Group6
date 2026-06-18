using Microsoft.EntityFrameworkCore;
using PCMS_Backend.Data;
using PCMS_Backend.Models;
using PCMS_Backend.Interfaces.Repositories;

namespace PCMS_Backend.Repositories;

public class CoverageScheduleRepository : ICoverageScheduleRepository
{
    private readonly PcmsDbContext _context;

    public CoverageScheduleRepository(PcmsDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CoverageSchedule>> GetAllAsync()
    {
        return await _context.CoverageSchedules
            .AsNoTracking()
            .OrderByDescending(cs => cs.WeekStartDate)
            .ToListAsync();
    }
}