namespace PCMS_Backend.DTOs;

public class AvailableSwapTargetDto
{
    public int CoverageAssignmentId { get; set; }

    public string Date { get; set; } = default!;

    public string Shift { get; set; } = default!;

    public string Specialty { get; set; } = default!;

    public string PhysicianName { get; set; } = default!;

    public int PhysicianId { get; set; }
}