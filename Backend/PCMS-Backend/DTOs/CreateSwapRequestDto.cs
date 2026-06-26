namespace PCMS_Backend.DTOs;

public class CreateSwapRequestDto
{
    public int RequestedPhysicianCoverageAssignmentId { get; set; }
    public int TargetedPhysicianCoverageAssignmentId { get; set; }

    public int TargetPhysicianId { get; set; }

    public string RequestComments { get; set; } = default!;
}