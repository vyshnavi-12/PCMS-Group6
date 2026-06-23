using PCMS_Backend.Shared; 
namespace PCMS_Backend.Interfaces.Services;
public interface ICoverageAssignmentsService
{    
    Task<Result> MarkAssignmentUnavailableAsync(int assignmentId, string reason, int physicianId);
}