namespace PCMS_Backend.DTOs;

public class CreateSwapRequestDto
{
    public int CoverageAssignmentId { get; set; }

    public int TargetPhysicianId { get; set; }

    public string RequestComments { get; set; } = default!;
}