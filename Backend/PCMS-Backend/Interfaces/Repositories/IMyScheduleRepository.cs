using PCMS_Backend.Models;

namespace PCMS_Backend.Interfaces.Repositories;

public interface IMyScheduleRepository
{
    Task<IReadOnlyList<CoverageAssignment>> GetPhysicianAssignmentsAsync(int physicianId);
}