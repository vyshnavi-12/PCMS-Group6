using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Services;

public class SupervisorService : ISupervisorService
{
    private readonly ISupervisorRepository _supervisorRepo;
    private readonly ISwapRequestRepository _swapRequestRepository;
    private readonly ICoverageScheduleRepository _coverageScheduleRepository;
    private readonly IPhysicianRepository _physicianRepository;

    public SupervisorService(ISupervisorRepository supervisorRepo, ISwapRequestRepository swapRequestRepository,
        ICoverageScheduleRepository coverageScheduleRepository, IPhysicianRepository physicianRepository)
    {
        _supervisorRepo = supervisorRepo;
        _swapRequestRepository = swapRequestRepository;
        _coverageScheduleRepository = coverageScheduleRepository;
        _physicianRepository = physicianRepository;
    }

    public async Task<Result<SupervisorDashboardDetailsDto>> DashboardDetails()
    {
        var pendingApproval =
            await _swapRequestRepository.GetPendingApprovalSwapRequestCount();

        var pendingUnavailable =
            await _physicianRepository.GetUnavailableRequestsCountSupervisorAsync();

        var today = DateOnly.FromDateTime(DateTime.Today);

        int daysUntilSaturday =
            ((int)DayOfWeek.Saturday - (int)today.DayOfWeek + 7) % 7;

        if (daysUntilSaturday == 0)
            daysUntilSaturday = 7;

        var nextScheduleDate = today.AddDays(daysUntilSaturday);

        var response = new SupervisorDashboardDetailsDto
        {
            SwapRequestCount = pendingApproval,
            UnavailableRequestsCount = pendingUnavailable,
            NextScheduleDate = nextScheduleDate
        };

        return Result<SupervisorDashboardDetailsDto>.Ok(response);
    }


}