using Microsoft.EntityFrameworkCore;
using PCMS_Backend.Data;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Models;
using System;

namespace PCMS_Backend.Repositories;

public class SupervisorRepository : ISupervisorRepository
{
    private readonly PcmsDbContext _db;

    public SupervisorRepository(PcmsDbContext db)
    {
        _db = db;
    }

    public async Task<List<CoverageGapAlert>> GetOpenGapAlertsAsync()
    {
        return await _db.CoverageGapAlerts
            .Include(cga => cga.CoverageAssignment)
                .ThenInclude(ca => ca.Specialty)
            .Where(cga => cga.AlertStatus == "Open")
            .OrderBy(cga => cga.CreatedAt)
            .ToListAsync();
    }
}