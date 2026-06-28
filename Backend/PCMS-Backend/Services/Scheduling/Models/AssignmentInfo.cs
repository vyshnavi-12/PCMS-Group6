namespace PCMS_Backend.Services.Scheduling.Models;

public class AssignmentInfo
{
    public DateOnly CoverageDate { get; set; }

    public string ShiftType { get; set; } = default!;

    public int SpecialtyId { get; set; }
}