using PCMS_Backend.DTOs;
using PCMS_Backend.Models;

namespace PCMS_Backend.Interfaces.Repositories;

public interface ICoverageScheduleRepository
{
    Task<IReadOnlyList<CoverageSchedule>> GetAllAsync();

    Task<CoverageSchedule?> GetByIdAsync(int scheduleId);

    Task<List<Physician>> GetPhysiciansAsync();
    Task<List<int>> GetSpecialtiesAsync();
    Task<List<ExternalLeavesData>> GetLeavesAsync();
    Task<List<ExternalShiftsData>> GetExternalShiftsAsync();
    Task<Dictionary<int, int>> GetWorkloadAsync();

    Task<CoverageSchedule> CreateScheduleAsync(DateTime startDate, int userId);
    Task SaveAssignmentsAsync(List<CoverageAssignment> assignments);

    Task<CoverageSchedule> GetScheduleWithAssignmentsAsync(int id);
    Task<DateOnly?> GetLastCreatedScheduleDateAsync();
    Task<CoverageSchedule?> GetByStartDateWithAssignmentsAsync(DateOnly startDate);
    Task<List<PhysicianWorkloadDto>> GetPhysicianWorkloadLast60DaysAsync(DateTime fromDate);

    Task SaveChangesAsync();
    Task<CoverageAssignment?> GetAssignmentByIdAsync(int assignmentId);

}