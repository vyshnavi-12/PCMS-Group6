using PCMS_Backend.Data;
using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Shared;
using Microsoft.EntityFrameworkCore;

namespace PCMS_Backend.Services;

public class MyScheduleService : IMyScheduleService
{
    private readonly IMyScheduleRepository _repository;
    private readonly IPhysicianRepository _physicianRepository;
    private readonly PcmsDbContext _context;

    public MyScheduleService(
    IMyScheduleRepository repository,
    IPhysicianRepository physicianRepository,
    PcmsDbContext context)
    {
        _repository = repository;
        _physicianRepository = physicianRepository;
        _context = context;
    }

    public async Task<Result<IReadOnlyList<DoctorScheduleDto>>> GetMyScheduleAsync(int userId)
    {
        var physician = await _physicianRepository.GetByUserIdAsync(userId);

        if (physician is null)
        {
            return Result<IReadOnlyList<DoctorScheduleDto>>
                .NotFound("Physician not found.");
        }

        var assignments = await _repository
            .GetPhysicianAssignmentsAsync(physician.PhysicianId);

        var response = new List<DoctorScheduleDto>();

        foreach (var a in assignments)
        {
            var swapRequest = await _context.SwapRequests
                .Where(s =>
                    s.RequestedPhysicianCoverageAssignmentId == a.CoverageAssignmentId ||
                    s.TargetedPhysicianCoverageAssignmentId == a.CoverageAssignmentId
                )
                .OrderByDescending(s => s.RequestedAt)
                .FirstOrDefaultAsync();

            string? swapStatus = swapRequest?.RequestStatus;

            bool canRequestSwap =
                swapRequest == null ||
                swapStatus == "TARGET_DECLINED" ||
                swapStatus == "REQUEST_REJECTED";

            response.Add(new DoctorScheduleDto
            {
                CoverageAssignmentId = a.CoverageAssignmentId,
                CoverageScheduleId = a.CoverageScheduleId,
                Date = a.CoverageDate,
                WeekStartDate = a.CoverageSchedule.WeekStartDate,
                WeekEndDate = a.CoverageSchedule.WeekEndDate,

                Shift = a.ShiftType,
                Specialty = a.Specialty.SpecialtyName,
                Time = a.ShiftType == "Day"
                    ? "06:00 AM - 06:00 PM"
                    : "06:00 PM - 06:00 AM",
                Status = a.AssignmentStatus == "Active" ? "ASSIGNED" : "PENDING",

                SwapRequestStatus = swapStatus,
                CanRequestSwap = canRequestSwap
            });
        }

        return Result<IReadOnlyList<DoctorScheduleDto>>
            .Ok(response);
    }
}