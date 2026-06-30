using Microsoft.EntityFrameworkCore;
using PCMS_Backend.Data;
using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Models;

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

    public async Task<CoverageSchedule?> GetByIdAsync(int scheduleId)
    {
        return await _context.CoverageSchedules
            .AsNoTracking()
            .Include(cs => cs.CoverageAssignments)
                .ThenInclude(ca => ca.Specialty)
            .Include(cs => cs.CoverageAssignments)
                .ThenInclude(ca => ca.Physician)
                    .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(cs =>
                cs.CoverageScheduleId == scheduleId);
    }


    public async Task<List<Physician>> GetPhysiciansAsync()
            => await _context.Physicians.Include(p => p.PhysicianSpecialtyMaps).Include(p => p.User).ToListAsync();

    public async Task<List<int>> GetSpecialtiesAsync()
        => await _context.Specialties.Select(s => s.SpecialtyId).ToListAsync();

    public async Task<List<ExternalLeavesData>> GetLeavesAsync()
        => await _context.ExternalLeavesData.ToListAsync();

    public async Task<List<ExternalShiftsData>> GetExternalShiftsAsync()
        => await _context.ExternalShiftsData.ToListAsync();

    public async Task<Dictionary<int, int>> GetWorkloadAsync()
    {
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-60));

        return await _context.CoverageAssignments
            .Where(a => a.CoverageDate >= fromDate)
            .GroupBy(a => a.PhysicianId)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }

    public async Task<CoverageSchedule> CreateScheduleAsync(DateTime start, int userId)
    {
        var schedule = new CoverageSchedule
        {
            ScheduleName = $"Week of {start:yyyy-MM-dd}",
            WeekStartDate = DateOnly.FromDateTime(start),
            WeekEndDate = DateOnly.FromDateTime(start.AddDays(6)),
            Status = "Draft",
            //hard coded , neeed to change ,fetch from jwt claims
            PublishedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.CoverageSchedules.Add(schedule);
        await _context.SaveChangesAsync();

        return schedule;
    }

    public async Task SaveAssignmentsAsync(List<CoverageAssignment> assignments)
    {
        _context.CoverageAssignments.AddRange(assignments);
        await _context.SaveChangesAsync();
    }

    public async Task<CoverageSchedule> GetScheduleWithAssignmentsAsync(int id)
    {
        return await _context.CoverageSchedules
            .Include(cs => cs.CoverageAssignments)
                .ThenInclude(ca => ca.Physician)
                    .ThenInclude(p => p.User)
            .FirstAsync(cs => cs.CoverageScheduleId == id);
    }
    public async Task<DateOnly?> GetLastCreatedScheduleDateAsync()
    {
        return await _context.CoverageSchedules
            .OrderByDescending(s => s.WeekStartDate)
            .Select(s => (DateOnly?)s.WeekStartDate)
            .FirstOrDefaultAsync();
    }

    public async Task<CoverageSchedule?> GetLatestPublishedScheduleAsync()
    {
        return await _context.CoverageSchedules
            .Where(x => x.Status == "Published")
            .OrderByDescending(x => x.WeekEndDate)
            .FirstOrDefaultAsync();
    }

    public async Task<CoverageSchedule?> GetByStartDateWithAssignmentsAsync(DateOnly startDate)
    {
        return await _context.CoverageSchedules
            .Include(s => s.CoverageAssignments)
            .FirstOrDefaultAsync(s => s.WeekStartDate == startDate);
    }

    public async Task<int> GetScheduleIdByStartDate(DateOnly startDate)
    {
        return await _context.CoverageSchedules
            .Where(c => c.WeekStartDate == startDate)
            .Select(c => c.CoverageScheduleId)
            .FirstOrDefaultAsync();
    }
    public async Task<List<PhysicianWorkloadDto>> GetPhysicianWorkloadLast60DaysAsync(DateTime fromDate)
    {
        return await _context.Physicians
            .Select(p => new PhysicianWorkloadDto
            {
                PhysicianId = p.PhysicianId,
                JoinDate = p.User.CreatedAt,

                MorningShiftCount = _context.CoverageAssignments
                    .Count(a =>
                        a.PhysicianId == p.PhysicianId &&
                        a.CoverageDate >= DateOnly.FromDateTime(fromDate) &&
                        a.ShiftType == "Day"),

                NightShiftCount = _context.CoverageAssignments
                    .Count(a =>
                        a.PhysicianId == p.PhysicianId &&
                        a.CoverageDate >= DateOnly.FromDateTime(fromDate) &&
                        a.ShiftType == "Night")
            })
            .ToListAsync();
    }

    public async Task<List<TopPhysicianRawDto>> GetTopPhysiciansPerSpecialtyRawAsync(int scheduleId)
    {
        var workloadData = await _context.CoverageAssignments
            .Where(a => a.CoverageScheduleId == scheduleId)
            .GroupBy(a => new
            {
                a.SpecialtyId,
                SpecialtyName = a.Specialty.SpecialtyName,
                a.PhysicianId,
                PhysicianName = a.Physician.User.FullName
            })
            .Select(g => new TopPhysicianRawDto
            {
                SpecialtyId = g.Key.SpecialtyId,
                SpecialtyName = g.Key.SpecialtyName,
                PhysicianId = g.Key.PhysicianId,
                PhysicianName = g.Key.PhysicianName,
                TotalAssignments = g.Count()
            })
            .ToListAsync();

        var result = workloadData
            .GroupBy(x => new { x.SpecialtyId, x.SpecialtyName })
            .Select(g => g
                .OrderByDescending(x => x.TotalAssignments)
                .First()
            )
            .ToList();

        return result;
    }

    public async Task<List<AssignmentRawDto>> GetAssignmentsByPhysiciansAsync(int scheduleId, List<int> physicianIds)
    {
        return await _context.CoverageAssignments
            .Where(a => a.CoverageScheduleId == scheduleId &&
                        physicianIds.Contains(a.PhysicianId))
            .Select(a => new AssignmentRawDto
            {
                PhysicianId = a.PhysicianId,
                SpecialtyId = a.SpecialtyId,
                Date = a.CoverageDate,
                ShiftType = a.ShiftType
            })
            .ToListAsync();
    }

    public async Task<List<AssignmentInfoDto>> GetActiveAssignmentsByPhysicianIdAsync(int physicianId)
    {
        var yesterday = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

        return await _context.CoverageAssignments
            .Where(a => a.PhysicianId == physicianId &&
                        a.CoverageDate >= yesterday)
            .Select(a => new AssignmentInfoDto
            {
               
                Date = a.CoverageDate,
                ShiftType = a.ShiftType
            })
            .ToListAsync();
    }



    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<CoverageAssignment?> GetAssignmentByIdAsync(int assignmentId)
    {
        return await _context.CoverageAssignments
            .FirstOrDefaultAsync(a => a.CoverageAssignmentId == assignmentId);
    }

    public async Task<int> GetCurrentScheduleId()
    {
        return await _context.CoverageSchedules
            .Where(cs => cs.Status == "Published")
            .OrderBy(cs => cs.WeekStartDate)
            .Select(cs => cs.CoverageScheduleId)
            .FirstOrDefaultAsync();
    }

}