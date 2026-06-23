namespace PCMS_Backend.DTOs;

public class GetAssignmentsDTO
{
    public int CoverageAssignmentId { get; set; }

    public int CoverageScheduleId { get; set; }
    public string ScheduleName { get; set; } = default!; // Added for UI display

    public DateOnly CoverageDate { get; set; }

    public int SpecialtyId { get; set; }
    public string SpecialtyName { get; set; } = default!; // Added for UI display

    public int PhysicianId { get; set; }

    public string ShiftType { get; set; } = default!;

    public string AssignmentStatus { get; set; } = default!;

    public DateTime CreatedAt { get; set; }
}