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

    public async Task CreateAlertAsync(CoverageGapAlertDto alert, int physicianId)
    {
        var alertEntity = new CoverageGapAlert
        {
            CoverageAssignmentId = alert.CoverageAssignmentId,
            CreatedAt = DateTime.UtcNow,
            AlertReason = alert.AlertReason,
            AlertStatus = alert.AlertStatus,
            RequestedByPhysicianId = physicianId

        };

        await _context.CoverageGapAlerts.AddAsync(alertEntity);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<OpenAlertsResponseDto>> GetOpenAlertsAsync()
    {
        var currentScheduleWeek = await _context.CoverageSchedules
            .Where(s => s.Status == "Published")
            .Select(s => new { startDate = s.WeekStartDate, endDate = s.WeekEndDate })
            .FirstOrDefaultAsync();
        if (currentScheduleWeek == null) return new List<OpenAlertsResponseDto>();
        DateTime startDateTime = currentScheduleWeek.startDate.ToDateTime(TimeOnly.MinValue)!;
        DateTime endDateTime = currentScheduleWeek.endDate.ToDateTime(TimeOnly.MinValue)!;

        var openAlerts = await _context.CoverageGapAlerts
            .Where(a => a.AlertStatus == "Open" && startDateTime <= a.CreatedAt.Date && a.CreatedAt.Date <= endDateTime)
            .Select(a => new OpenAlertsResponseDto
            {
                Date = a.CoverageAssignment.CoverageDate,
                Specialty = a.CoverageAssignment.Specialty.SpecialtyName,
                Shift = a.CoverageAssignment.ShiftType,
                RequestedBy = a.CoverageAssignment.Physician.User.FullName,
                Status = a.AlertStatus,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();
        return openAlerts;
    }

    public async Task<IReadOnlyList<OpenAlertsResponseDto>> GetAlertsAsync()
    {
        bool hasPublishedSchedule = await _context.CoverageSchedules
            .AnyAsync(s => s.Status == "Published");

        if (!hasPublishedSchedule)
            return new List<OpenAlertsResponseDto>();

        DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

        // Fetch all gaps from today forward
        var openAlerts = await _context.CoverageGapAlerts
            .Where(a => a.CoverageAssignment.CoverageDate >= currentDate)
            .Select(a => new OpenAlertsResponseDto
            {
                AlertId = a.CoverageGapAlertId,
                Date = a.CoverageAssignment.CoverageDate,
                Specialty = a.CoverageAssignment.Specialty.SpecialtyName,
                Shift = a.CoverageAssignment.ShiftType,
                RequestedBy = a.AlertStatus == "Open" ? a.CoverageAssignment.Physician.User.FullName : a.Physician.User.FullName,
                Status = a.AlertStatus,
                CreatedAt = a.CreatedAt
            })
            .OrderBy(a => a.Date) // Most urgent/immediate gaps show up first!
            .ToListAsync();

        return openAlerts;
    }

    public async Task<CoverageGapAlert?> GetAlertDetailsByIdAsync(int alertId)
    {
        return await _context.CoverageGapAlerts
            .FirstOrDefaultAsync(a => a.CoverageGapAlertId == alertId);
    }
    public async Task<bool> UpdateAlertStatusToResolvedAsync(int alertId)
    {
        var alert = await _context.CoverageGapAlerts.FindAsync(alertId);
        if (alert == null) return false;
        alert.ResolvedAt = DateTime.UtcNow;        
        alert.AlertStatus = "Resolved";
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> UpdateAssignmentPhysicianAsync(int alertId, int physicianId)
    {
        var alert = await _context.CoverageGapAlerts
            .Include(a => a.CoverageAssignment)
            .FirstOrDefaultAsync(a => a.CoverageGapAlertId == alertId);
        if (alert == null || alert.CoverageAssignment == null) return false;
            alert.CoverageAssignment.PhysicianId = physicianId;
            await _context.SaveChangesAsync();
        return true;
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }


    public async Task<IReadOnlyList<UnavailableRequestsPerSpecialtyDto>> GetUnavailableRequestsPerSpecialtyAsync()
    {
        return await _context.CoverageGapAlerts
            .Include(cga => cga.CoverageAssignment)
            .ThenInclude(ca => ca.Specialty)
            .GroupBy(cga => cga.CoverageAssignment.Specialty.SpecialtyName)
            .Select(group => new UnavailableRequestsPerSpecialtyDto
            {
                SpecialtyName = group.Key,
                RequestCount = group.Count()
            })
            .OrderByDescending(dto => dto.RequestCount)
            .ToListAsync();
    }

    public async Task<int?> GetAssignmentIdByAlertIdAsync(int alertId)
    {
        var alert = await _context.CoverageGapAlerts.Where(cga => cga.CoverageGapAlertId == alertId).FirstOrDefaultAsync();
        if (alert == null) return null;
        return alert.CoverageAssignmentId;
    }

    public async Task<bool> ChangeAssignmentStatus(int assignmentId, string status)
    {
        var assignment = await _context.CoverageAssignments.Where(ca => ca.CoverageAssignmentId == assignmentId).FirstOrDefaultAsync();
        if (assignment == null) return false;
        assignment.AssignmentStatus = status;
        await _context.SaveChangesAsync();
        return true;
    }

}