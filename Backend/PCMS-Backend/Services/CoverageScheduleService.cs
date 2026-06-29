using Microsoft.AspNetCore.SignalR;
using PCMS_Backend.DTOs;
using PCMS_Backend.Hubs;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Models;
using PCMS_Backend.Models.Scheduling;
using PCMS_Backend.Services.Scheduling.Extensions;
using PCMS_Backend.Services.Scheduling.Interfaces;
using PCMS_Backend.Services.Scheduling.Models;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Services;

public class CoverageScheduleService : ICoverageScheduleService
{
    private readonly ICoverageScheduleRepository _coverageScheduleRepository;
    private readonly INotificationService _notificationService;
    private readonly IHubContext<ScheduleHub> _scheduleHubContext;
    private readonly IAuditLogService _auditLogService;
    private readonly IRecommendationContextBuilder _contextBuilder;
    private readonly IPhysicianRecommendationService _recommendationService;



    public CoverageScheduleService(
       ICoverageScheduleRepository coverageScheduleRepository,
       INotificationService notificationService,
       IAuditLogService auditLogService,
       IHubContext<ScheduleHub> scheduleHubContext,
       IRecommendationContextBuilder contextBuilder,
       IPhysicianRecommendationService recommendationService)
    {
        _coverageScheduleRepository = coverageScheduleRepository;
        _notificationService = notificationService;
        _auditLogService = auditLogService;   // ✅ assign
        _scheduleHubContext = scheduleHubContext;
        _recommendationService = recommendationService;
        _contextBuilder = contextBuilder;
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

    private DateTime GetCurrStartDate(DateTime today)
    {
        int daysToAdd = ((int)DayOfWeek.Monday - (int)today.DayOfWeek);




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


    public async Task<Result<List<TopPhysicianPerSpecialtyDto>>> GetTopPerSpecialty()
    {

        int scheduleId = await _coverageScheduleRepository.GetCurrentScheduleId();

        var topData = await _coverageScheduleRepository.GetTopPhysiciansPerSpecialtyRawAsync(scheduleId);

        var physicianIds = topData
            .Select(x => x.PhysicianId)
            .ToList();


        var assignments = await _coverageScheduleRepository.GetAssignmentsByPhysiciansAsync(scheduleId, physicianIds);


        var result = topData.Select(x => new TopPhysicianPerSpecialtyDto
        {
            PhysicianId = x.PhysicianId,
            PhysicianName = x.PhysicianName,

            SpecialtyId = x.SpecialtyId,
            SpecialtyName = x.SpecialtyName,

            TotalAssignments = x.TotalAssignments,

            Assignments = assignments
                .Where(a => a.PhysicianId == x.PhysicianId &&
                            a.SpecialtyId == x.SpecialtyId)
                .Select(a => new AssignmentInfoDto
                {
                    Date = a.Date,
                    ShiftType = a.ShiftType
                })
                .ToList()

        }).ToList();

        return Result<List<TopPhysicianPerSpecialtyDto>>
            .Ok(result, "Top physicians per specialty fetched successfully");
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
                    PhysicianId = ca.PhysicianId,
                    PhysicianName = ca.Physician.User.FullName,
                    ShiftType = ca.ShiftType,
                    AssignmentStatus = ca.AssignmentStatus
                })
                .ToList()
        };

        return Result<CoverageScheduleDetailDto>
            .Ok(response);
    }

    public async Task<Result<CoverageScheduleGenerateResponseDto>> GenerateScheduleAsync(int userId)
    {
        var startDate = GetNextStartDate(DateTime.UtcNow);
        var startDateOnly = DateOnly.FromDateTime(startDate);
        var slots = GenerateSlots(startDate);
        var specialties = await _coverageScheduleRepository.GetSpecialtiesAsync();

        int total = slots.Count * specialties.Count;
        var uncoveredSlots = new List<UncoveredSlotDto>();

        // ---------------------------------------------------------
        // 1. Check for Existing Schedule
        // ---------------------------------------------------------
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
                    CoveragePercentage = (double)existingSchedule.CoverageAssignments.Count / total * 100,
                    UncoveredSlots = uncoveredSlots
                }
            );
        }

        // ---------------------------------------------------------
        // 2. Build the Scheduling Context
        // ---------------------------------------------------------
        // The builder handles fetching physicians, leaves, shifts, 
        // and calculates the 60-day normalized workload baseline.
        var context = await _contextBuilder.BuildAsync(startDateOnly);

        var assignments = new List<CoverageAssignment>();

        // ---------------------------------------------------------
        // 3. Engine-Driven Assignment Loop
        // ---------------------------------------------------------
        foreach (var slot in slots)
        {
            foreach (var spec in specialties)
            {
                var request = new RecommendationRequest
                {
                    CoverageDate = slot.Date,
                    ShiftType = slot.ShiftType,
                    SpecialtyId = spec,
                    AllowLimitOverride = false
                };

                // Get recommendations (Handles Leaves, Gaps, and Primary/Secondary filtering internally)
                var recommendations = await _recommendationService.GetRecommendations(request, context);
                var bestCandidate = recommendations.FirstOrDefault();

                // Fallback: If no one is available because of Weekly Limits, override limits and try again.
                if (bestCandidate == null)
                {
                    request.AllowLimitOverride = true;
                    var fallbackRecs = await _recommendationService.GetRecommendations(request, context);
                    bestCandidate = fallbackRecs.FirstOrDefault();
                }

                // Assign or mark as uncovered
                if (bestCandidate != null)
                {
                    assignments.Add(new CoverageAssignment
                    {
                        CoverageDate = slot.Date,
                        ShiftType = slot.ShiftType,
                        SpecialtyId = spec,
                        PhysicianId = bestCandidate.PhysicianId,
                        AssignmentStatus = "Assigned"
                    });

                    // CRITICAL: Update the state in real-time so the RestGap and WeeklyLimit rules 
                    // know about this assignment during the next iteration of the loop.
                    context.RegisterAssignment(bestCandidate.PhysicianId, slot.Date, slot.ShiftType, spec);
                }
                else
                {
                    // Uncovered because everyone is on leave, on an external shift, or restricted by rest-gaps.
                    uncoveredSlots.Add(new UncoveredSlotDto
                    {
                        Date = slot.Date,
                        ShiftType = slot.ShiftType,
                        SpecialtyId = spec
                    });
                }
            }
        }

        // ---------------------------------------------------------
        // 4. Save and Return
        // ---------------------------------------------------------
        var schedule = await _coverageScheduleRepository.CreateScheduleAsync(startDate, userId);

        foreach (var a in assignments)
        {
            a.CoverageScheduleId = schedule.CoverageScheduleId;
        }

        await _coverageScheduleRepository.SaveAssignmentsAsync(assignments);

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

        if (schedule is null)
            return Result<bool>.NotFound("Schedule not found.");

        if (schedule.Status == "Published")
            return Result<bool>.BadRequest("Schedule is already published.");

        schedule.Status = "Published";
        schedule.PublishedAt = DateTime.UtcNow;
        schedule.PublishedByUserId = userId;

        foreach (var a in schedule.CoverageAssignments)
        {
            a.AssignmentStatus = "Active";
        }

        await _coverageScheduleRepository.SaveChangesAsync();

        var assignedUserIds = schedule.CoverageAssignments
            .Select(a => a.Physician.UserId)
            .Distinct()
            .ToList();

        foreach (var assignedUserId in assignedUserIds)
        {
            await _scheduleHubContext.Clients
                .Group($"User_{assignedUserId}")
                .SendAsync("SchedulePublished", new
                {
                    scheduleId = schedule.CoverageScheduleId,
                    message = "Schedule Published"
                });
        }

        await _notificationService.CreateSchedulePublishedNotificationsAsync(schedule);

        // ✅ Audit log entry
        await _auditLogService.LogActionAsync(
            "PublishSchedule",
            "CoverageSchedule",
            schedule.CoverageScheduleId,
            userId
        );

        return Result<bool>.Ok(true);
    }

            

    public async Task<Result> UpdateAssignmentsAsync(
    int scheduleId,
    UpdateCoverageAssignmentsDto dto)
    {
        var schedule =
            await _coverageScheduleRepository
                .GetScheduleWithAssignmentsAsync(scheduleId);

        if (schedule == null)
        {
            return Result.NotFound("Schedule not found.");
        }

        if (schedule.Status.Equals("Published", StringComparison.OrdinalIgnoreCase))
        {
            return Result.BadRequest("Published schedules cannot be modified.");
        }

        foreach (var update in dto.Assignments)
        {   
            var assignment =
                schedule.CoverageAssignments
                    .FirstOrDefault(a =>
                        a.CoverageAssignmentId ==
                        update.CoverageAssignmentId);

            if (assignment == null)
            {
                continue;
            }

            if (assignment.PhysicianId != update.PhysicianId)
            {
                assignment.PhysicianId = update.PhysicianId;
            }
        }

        
            await _coverageScheduleRepository
                .SaveChangesAsync();

        return Result.Ok("Schedule updated successfully.");
    }
}

