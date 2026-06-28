using PCMS_Backend.DTOs;
using PCMS_Backend.Models;

public interface IRecommendationRepository
{
    Task<List<Physician>> GetPhysiciansAsync();

    Task<List<ExternalLeavesData>> GetLeavesAsync();

    Task<List<ExternalShiftsData>> GetExternalShiftsAsync();

    Task<List<CoverageAssignment>> GetAssignmentsAsync(
        DateOnly startDate,
        DateOnly endDate);

    Task<List<PhysicianWorkloadDto>> GetPhysicianWorkloadsAsync();
}