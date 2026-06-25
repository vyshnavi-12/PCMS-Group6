using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Models;
using PCMS_Backend.Repositories;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Services;

public class PhysicianService : IPhysicianService
{
    private readonly IPhysicianRepository _physicianRepo;
    private readonly ICoverageScheduleRepository _coverageScheduleRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PhysicianService(IPhysicianRepository physicianRepo, IHttpContextAccessor httpContextAccessor, ICoverageScheduleRepository coverageScheduleRepository)
    {
        _physicianRepo = physicianRepo;
        _httpContextAccessor = httpContextAccessor;
        _coverageScheduleRepository = coverageScheduleRepository;
    }

    public async Task<Result<IReadOnlyList<GetAssignmentsDTO>>> GetAllAssignmentsAsync(int userId)
    {
        var assignments = await _physicianRepo.GetAssignmentsByUserIdAsync(userId);

        if (assignments == null || !assignments.Any())
        {
            return Result<IReadOnlyList<GetAssignmentsDTO>>.Ok(
                new List<GetAssignmentsDTO>(),
                "No assignments found."
            );
        }

        var dtoList = assignments.Select(a => new GetAssignmentsDTO
        {
            CoverageAssignmentId = a.CoverageAssignmentId,
            CoverageScheduleId = a.CoverageScheduleId ,
            ScheduleName = a.CoverageSchedule?.ScheduleName ?? "Unpublished",
            CoverageDate = a.CoverageDate,
            SpecialtyId = a.SpecialtyId,
            SpecialtyName = a.Specialty?.SpecialtyName ?? "Unknown",
            PhysicianId = a.PhysicianId,
            ShiftType = a.ShiftType,
            AssignmentStatus = a.AssignmentStatus,
            CreatedAt = a.CreatedAt 
        }).ToList();

        return Result<IReadOnlyList<GetAssignmentsDTO>>.Ok(dtoList, "Assignments retrieved successfully.");
    }
    public async Task<int?> GetPhysicianIdByUserIdAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User.GetCurrentUserId();
        if (userId is not int validUserId) return null;
        var physicianId = await _physicianRepo.GetPhysicianIdByUserIdAsync(validUserId);
        if (physicianId is not null) return physicianId;
        return null;
    }
    public async Task<Result<IReadOnlyList<ReplacementPhysicianDto>>> GetSuggestedReplacementsAsync(int assignmentId)
    {
        // 1. Fetch the assignment that needs a replacement
        var assignment = await _coverageScheduleRepository.GetAssignmentByIdAsync(assignmentId);
        if (assignment == null)
            return Result<IReadOnlyList<ReplacementPhysicianDto>>.NotFound("Assignment not found.");

        int requiredSpecialty = assignment.SpecialtyId;
        DateTime targetDate = assignment.CoverageDate.ToDateTime(TimeOnly.MinValue); string targetShift = assignment.ShiftType;
        int unavailablePhysicianId = assignment.PhysicianId;

        // 2. Fetch baseline data
        var schedule = await _coverageScheduleRepository.GetByStartDateWithAssignmentsAsync(DateOnly.FromDateTime(targetDate));
        var physicians = await _coverageScheduleRepository.GetPhysiciansAsync();
        var leaves = await _coverageScheduleRepository.GetLeavesAsync();
        var shifts = await _coverageScheduleRepository.GetExternalShiftsAsync();

        // 3. Workload calculations (Last 60 days)
        var fromDate = DateTime.UtcNow.AddDays(-60);
        var workloadData = await _coverageScheduleRepository.GetPhysicianWorkloadLast60DaysAsync(fromDate);

        int avgWorkload = 0, avgNightWorkload = 0;
        var oldDoctors = workloadData.Where(d => d.JoinDate <= fromDate).ToList();

        if (oldDoctors.Count > 0)
        {
            avgWorkload = (int)oldDoctors.Average(d => d.MorningShiftCount);
            avgNightWorkload = (int)oldDoctors.Average(d => d.NightShiftCount);
        }

        var normalizedWorkload = new Dictionary<int, int>();
        foreach (var d in workloadData)
        {
            int baseWork = d.JoinDate > fromDate ? avgWorkload : (targetShift == "Night" ? d.NightShiftCount : d.MorningShiftCount);
            int avgToUse = targetShift == "Night" ? avgNightWorkload : avgWorkload;
            normalizedWorkload[d.PhysicianId] = Math.Max(0, baseWork - avgToUse);
        }

        // 4. Calculate Current Week Load & Blocks from the existing schedule
        var currentWeekLoad = new Dictionary<int, int>();
        var currentNightLoad = new Dictionary<int, int>();
        var blockedDates = new Dictionary<int, HashSet<DateTime>>();

        if (schedule != null)
        {
            foreach (var a in schedule.CoverageAssignments.Where(a => a.AssignmentStatus != "Unavailable"))
            {
                if (!currentWeekLoad.ContainsKey(a.PhysicianId)) currentWeekLoad[a.PhysicianId] = 0;
                if (!currentNightLoad.ContainsKey(a.PhysicianId)) currentNightLoad[a.PhysicianId] = 0;
                if (!blockedDates.ContainsKey(a.PhysicianId)) blockedDates[a.PhysicianId] = new HashSet<DateTime>();

                currentWeekLoad[a.PhysicianId]++;
                if (a.ShiftType == "Night") currentNightLoad[a.PhysicianId]++;

                // Block the date so they aren't assigned twice on the same day
                blockedDates[a.PhysicianId].Add(a.CoverageDate.ToDateTime(TimeOnly.MinValue));
            }
        }

        int currWeekTotalLimit = 4;
        int currWeekNightLimit = 2;

        // Use a temporary Tuple list just to hold the math and sorting data
        var eligibleCandidates = new List<(int PhysicianId, string PhysicianName, bool IsPrimary, int Score)>();

        // 5. Filter and Score Candidates
        foreach (var p in physicians)
        {
            if (p.PhysicianId == unavailablePhysicianId) continue;

            var specialtyMap = p.PhysicianSpecialtyMaps.FirstOrDefault(s => s.SpecialtyId == requiredSpecialty);
            if (specialtyMap == null) continue;

            if (IsOnLeave(p.PhysicianId, DateOnly.FromDateTime(targetDate), leaves)) continue;
            if (HasExternalShift(p.PhysicianId, DateOnly.FromDateTime(targetDate), shifts)) continue;

            int weekShifts = currentWeekLoad.GetValueOrDefault(p.PhysicianId, 0);
            int nightShifts = currentNightLoad.GetValueOrDefault(p.PhysicianId, 0);

            if (weekShifts >= currWeekTotalLimit) continue;
            if (targetShift == "Night" && nightShifts >= currWeekNightLimit) continue;

            if (blockedDates.ContainsKey(p.PhysicianId) && blockedDates[p.PhysicianId].Contains(targetDate.Date)) continue;

            int pastWorkload = normalizedWorkload.GetValueOrDefault(p.PhysicianId, 0);
            int score = pastWorkload + weekShifts;

            // Add to the temporary Tuple list instead of the DTO
            eligibleCandidates.Add((
                p.PhysicianId,
                p.User?.FullName ?? "Unknown",
                specialtyMap.IsPrimarySpecialty,
                score
            ));
        }

        // 6. Sort using the Tuple data, then Select into your perfectly clean DTO!
        var sortedCandidates = eligibleCandidates
            .OrderByDescending(c => c.IsPrimary)
            .ThenBy(c => c.Score)
            .Select((c, index) => new ReplacementPhysicianDto
            {
                PhysicianId = c.PhysicianId,
                PhysicianName = c.PhysicianName,
                IsRecommended = index == 0 // Top 1 doctors get the star!
            })
            .ToList();

        return Result<IReadOnlyList<ReplacementPhysicianDto>>.Ok(sortedCandidates, "Replacements suggested successfully.");
    }
    private void RemoveAndReinsert(
        int doc,
        int slot,
        List<int> specialties,
        Dictionary<(int, int), HashSet<int>> map,
        PriorityQueue<(int, int), int> pq)
    {
        var affected = new[] { slot - 1, slot, slot + 1 };

        foreach (var s in affected)
        {
            foreach (var spec in specialties)
            {
                var key = (s, spec);

                if (!map.ContainsKey(key)) continue;

                if (map[key].Remove(doc))
                {
                    pq.Enqueue(key, map[key].Count);
                }
            }
        }
    }

    private void BlockDoctor(int doc, int slot, Dictionary<int, HashSet<int>> blocked)
    {
        if (!blocked.ContainsKey(doc))
            blocked[doc] = new HashSet<int>();

        blocked[doc].Add(slot);
        blocked[doc].Add(slot - 1);
        blocked[doc].Add(slot + 1);
    }

    private bool IsBlocked(int doc, int slot, Dictionary<int, HashSet<int>> blocked)
    {
        return blocked.ContainsKey(doc) && blocked[doc].Contains(slot);
    }

    private bool IsOnLeave(int doc, DateOnly date, List<ExternalLeavesData> leaves)
    {
        return leaves.Any(l =>
            l.PhysicianId == doc &&
            date >= l.LeaveStartDate &&
            date <= l.LeaveEndDate);
    }

    private bool HasExternalShift(int doc, DateOnly date, List<ExternalShiftsData> shifts)
    {
        return shifts.Any(s =>
            s.PhysicianId == doc &&
            s.ShiftDate == date);
    }
}