using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Models;
using PCMS_Backend.Shared;
using PCMS_Backend.Models.Scheduling;

namespace PCMS_Backend.Services;

public class CoverageScheduleService : ICoverageScheduleService
{
    private readonly ICoverageScheduleRepository _coverageScheduleRepository;

    public CoverageScheduleService(
        ICoverageScheduleRepository coverageScheduleRepository)
    {
        _coverageScheduleRepository = coverageScheduleRepository;
    }
    private List<Slot> GenerateSlots(DateTime startDate)
    {
        var slots = new List<Slot>();
        int index = 1;

        for (int i = 0; i < 7; i++)
        {
            var date = DateOnly.FromDateTime(startDate.AddDays(i));

            // ✅ Day Shift (6AM - 6PM)
            slots.Add(new Slot
            {
                Index = index++,
                Date = date,
                ShiftType = "Day"
            });

            // ✅ Night Shift (6PM - 6AM next day)
            slots.Add(new Slot
            {
                Index = index++,
                Date = date,
                ShiftType = "Night"
            });
        }

        return slots;
    }

    private DateTime GetNextStartDate(DateTime today)
    {
        int daysToAdd = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;

        // ✅ If today is Monday → move to next week
        if (daysToAdd == 0)
            daysToAdd = 7;
        

        return today.AddDays(daysToAdd).Date;
    }

    // =========================================
    // ✅ EXISTING METHODS (UNCHANGED)
    // =========================================

    public async Task<Result<IReadOnlyList<CoverageScheduleDto>>> GetAllSchedulesAsync()
    {
        var schedules = await _coverageScheduleRepository.GetAllAsync();

        var response = schedules
    .Select(cs => new CoverageScheduleDto
    {
        CoverageScheduleId = cs.CoverageScheduleId,
        ScheduleName = cs.ScheduleName,
        WeekStartDate = cs.WeekStartDate,
        WeekEndDate = cs.WeekEndDate,
        Status = cs.Status,
        PublishedAt = cs.PublishedAt
    })
    .ToList();

        return Result<IReadOnlyList<CoverageScheduleDto>>.Ok(response);
    }

    public async Task<Result<CoverageScheduleDetailDto>> GetScheduleByIdAsync(int scheduleId)
    {
        var schedule = await _coverageScheduleRepository.GetByIdAsync(scheduleId);

        if (schedule is null)
        {
            return Result<CoverageScheduleDetailDto>.NotFound("Schedule not found.");
        }

        var response = new CoverageScheduleDetailDto
        {
            CoverageScheduleId = schedule.CoverageScheduleId,
            ScheduleName = schedule.ScheduleName,
            WeekStartDate = schedule.WeekStartDate,
            WeekEndDate = schedule.WeekEndDate,
            Status = schedule.Status,

            Assignments = schedule.CoverageAssignments
                .Select(ca => new CoverageAssignmentDto
                {
                    CoverageAssignmentId = ca.CoverageAssignmentId,
                    CoverageDate = ca.CoverageDate,
                    SpecialtyName = ca.Specialty.SpecialtyName,
                    PhysicianName = ca.Physician.User.FullName,
                    ShiftType = ca.ShiftType,
                    AssignmentStatus = ca.AssignmentStatus
                })
                .ToList()
        };

        return Result<CoverageScheduleDetailDto>.Ok(response);
    }

    // =========================================
    // ✅ GENERATE SCHEDULE
    // =========================================

    public async Task<Result<CoverageScheduleGenerateResponseDto>> GenerateScheduleAsync(int userId)
    {
        var startDate = GetNextStartDate(DateTime.UtcNow);

        var startDateOnly = DateOnly.FromDateTime(startDate);
        var slots = GenerateSlots(startDate);
        var specialties = await _coverageScheduleRepository.GetSpecialtiesAsync();

        int total = slots.Count * specialties.Count;
        var uncoveredSlots = new List<UncoveredSlotDto>();

        var existingSchedule = await _coverageScheduleRepository
            .GetByStartDateWithAssignmentsAsync(startDateOnly);
        


        if (existingSchedule != null)
        {
            

            var assignedSet = existingSchedule.CoverageAssignments
                .Select(a => (a.CoverageDate, a.ShiftType, a.SpecialtyId))
                .ToHashSet();

            

            foreach (var slot in slots)
            {
                foreach (var spec in specialties)
                {
                    if (!assignedSet.Contains((slot.Date, slot.ShiftType, spec)))
                    {
                        uncoveredSlots.Add(new UncoveredSlotDto
                        {
                            Date = slot.Date,
                            ShiftType = slot.ShiftType,
                            SpecialtyId = spec
                        });
                    }
                }
            }

            return Result<CoverageScheduleGenerateResponseDto>.Ok(
                new CoverageScheduleGenerateResponseDto
                {
                    ScheduleId = existingSchedule.CoverageScheduleId,
                    AssignedSlots = existingSchedule.CoverageAssignments.Count,
                    TotalSlots = total,
                    CoveragePercentage =
                        (double)existingSchedule.CoverageAssignments.Count / total * 100,
                    UncoveredSlots = uncoveredSlots
                }
            );
        }
        var physicians = await _coverageScheduleRepository.GetPhysiciansAsync();

        var leaves = await _coverageScheduleRepository.GetLeavesAsync();
        var shifts = await _coverageScheduleRepository.GetExternalShiftsAsync();
        var workload = await _coverageScheduleRepository.GetWorkloadAsync();

        
       

        var availability = new Dictionary<(int, int), HashSet<int>>();
        var pq = new PriorityQueue<(int, int), int>();
        var assigned = new HashSet<(int, int)>();
        var blocked = new Dictionary<int, HashSet<int>>();
        var assignments = new List<CoverageAssignment>();
        var currentWorkload = new Dictionary<int, int>(workload);

        // ✅ PRIMARY AVAILABILITY
        foreach (var slot in slots)
        {
            foreach (var spec in specialties)
            {
                var available = physicians
                    .Where(p => p.PhysicianSpecialtyMaps
                        .Any(s => s.SpecialtyId == spec && s.IsPrimarySpecialty))
                    .Where(p => !IsOnLeave(p.PhysicianId, slot.Date, leaves))
                    .Where(p => !HasExternalShift(p.PhysicianId, slot.Date, shifts))
                    .Select(p => p.PhysicianId)
                    .ToHashSet();

                availability[(slot.Index, spec)] = available;
                pq.Enqueue((slot.Index, spec), available.Count);
            }
        }

        ProcessQueue(pq, availability, specialties, slots, assigned, blocked, workload, currentWorkload, assignments);

        // ✅ SECONDARY PASS
        var remaining = availability.Keys.Where(k => !assigned.Contains(k)).ToList();

        var availability2 = new Dictionary<(int, int), HashSet<int>>();
        var pq2 = new PriorityQueue<(int, int), int>();

        foreach (var key in remaining)
        {
            var slotObj = slots.First(s => s.Index == key.Item1);

            var available = physicians
                .Where(p => p.PhysicianSpecialtyMaps
                    .Any(s => s.SpecialtyId == key.Item2 && !s.IsPrimarySpecialty))
                .Where(p => !IsOnLeave(p.PhysicianId, slotObj.Date, leaves))
                .Where(p => !HasExternalShift(p.PhysicianId, slotObj.Date, shifts))
                .Where(p => !IsBlocked(p.PhysicianId, key.Item1, blocked))
                .Select(p => p.PhysicianId)
                .ToHashSet();

            availability2[key] = available;
            pq2.Enqueue(key, available.Count);
        }

        ProcessQueue(pq2, availability2, specialties, slots, assigned, blocked, workload, currentWorkload, assignments);

        // ✅ SAVE
        var schedule = await _coverageScheduleRepository.CreateScheduleAsync(startDate,userId);

        foreach (var a in assignments)
        {
            a.CoverageScheduleId = schedule.CoverageScheduleId;
            a.AssignmentStatus = "Draft";
        }

        await _coverageScheduleRepository.SaveAssignmentsAsync(assignments);


        foreach (var slot in slots)
        {
            foreach (var spec in specialties)
            {
                if (!assigned.Contains((slot.Index, spec)))
                {
                    uncoveredSlots.Add(new UncoveredSlotDto
                    {
                        Date = slot.Date,
                        ShiftType = slot.ShiftType,
                        SpecialtyId = spec
                    });
                }
            }
        }

        return Result<CoverageScheduleGenerateResponseDto>.Ok(
            new CoverageScheduleGenerateResponseDto
            {
                ScheduleId = schedule.CoverageScheduleId,
                AssignedSlots = assignments.Count,
                TotalSlots = total,
                CoveragePercentage = (double)assignments.Count / total * 100,
                UncoveredSlots = uncoveredSlots

            }
        );
    }

    // =========================================
    // ✅ PUBLISH
    // =========================================

    public async Task<Result<bool>> PublishScheduleAsync(int scheduleId, int userId)
    {
        var schedule = await _coverageScheduleRepository
            .GetScheduleWithAssignmentsAsync(scheduleId);

        // ✅ NOT FOUND
        if (schedule is null)
        {
            return Result<bool>.NotFound("Schedule not found.");
        }

        // ✅ ALREADY PUBLISHED CHECK
        if (schedule.Status == "Published")
        {
            return Result<bool>.BadRequest("Schedule is already published.");
        }

        // ======================================
        // ✅ PUBLISH
        // ======================================

        schedule.Status = "Published";
        schedule.PublishedAt = DateTime.UtcNow;
        schedule.PublishedByUserId = userId;

        foreach (var a in schedule.CoverageAssignments)
        {
            a.AssignmentStatus = "Active";
        }

        await _coverageScheduleRepository.SaveChangesAsync();

        return Result<bool>.Ok(true);
    }

    // =========================================
    // ✅ CORE ENGINE
    // =========================================

    private void ProcessQueue(
        PriorityQueue<(int, int), int> pq,
        Dictionary<(int, int), HashSet<int>> availability,
        List<int> specialties,
        List<Slot> slots,
        HashSet<(int, int)> assigned,
        Dictionary<int, HashSet<int>> blocked,
        Dictionary<int, int> workload,
        Dictionary<int,int>currWorkload,
        List<CoverageAssignment> result)
    {
        while (pq.Count > 0)
        {
            var key = pq.Dequeue();

            if (assigned.Contains(key)) continue;

            var available = availability[key];

            int selected = -1;
            int minLoad = int.MaxValue;

            foreach (var d in available)
            {
                if (IsBlocked(d, key.Item1, blocked)) continue;

                var load = workload.GetValueOrDefault(d, 0);

                if (load < minLoad)
                {
                    minLoad = load;
                    selected = d;
                }
            }

            if (selected == -1) continue;

            var slotObj = slots.First(s => s.Index == key.Item1);

            result.Add(new CoverageAssignment
            {
                PhysicianId = selected,
                SpecialtyId = key.Item2,
                CoverageDate = slotObj.Date,
                ShiftType = slotObj.ShiftType,
                CreatedAt = DateTime.UtcNow
            });

            if (!workload.ContainsKey(selected))
                workload[selected] = 0;

            workload[selected]++;


            assigned.Add(key);
            BlockDoctor(selected, key.Item1, blocked);

            RemoveAndReinsert(selected, key.Item1, specialties, availability, pq);
        }
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
