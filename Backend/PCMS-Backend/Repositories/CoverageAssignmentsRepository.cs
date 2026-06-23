using Microsoft.EntityFrameworkCore;
using PCMS_Backend.Data;
using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Models;

namespace PCMS_Backend.Repositories;

public class CoverageAssignmentsRepo : ICoverageAssignmentsRepository
{
    private readonly PcmsDbContext _context;

    public CoverageAssignmentsRepo(PcmsDbContext context)
    {
        _context = context;
    }

    public async Task<CoverageAssignment?> GetAssignmentByIdAsync(int assignmentId)
    {
        return await _context.CoverageAssignments
            .FirstOrDefaultAsync(c => c.CoverageAssignmentId == assignmentId);
    }

    public async Task CreateAlertAsync(CoverageGapAlertDto alert)
    {
        var alertEntity = new CoverageGapAlert
        {
            CoverageAssignmentId = alert.CoverageAssignmentId,
            CreatedAt = DateTime.UtcNow,
            AlertReason = alert.AlertReason,
            AlertStatus = alert.AlertStatus

        };

        await _context.CoverageGapAlerts.AddAsync(alertEntity);
        await _context.SaveChangesAsync();
    }

 
}