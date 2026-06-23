using PCMS_Backend.Models;

namespace PCMS_Backend.Interfaces.Repositories;

public interface ISwapRequestRepository
{
    Task<CoverageAssignment?> GetAssignmentByIdAsync(int coverageAssignmentId);

    Task<List<CoverageAssignment>> GetAvailableTargetsAsync(
      int specialtyId,
      int excludedPhysicianId,
      string shiftType
  );
}