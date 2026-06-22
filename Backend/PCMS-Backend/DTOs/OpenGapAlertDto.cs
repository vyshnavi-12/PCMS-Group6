namespace PCMS_Backend.DTOs;

public class OpenGapAlertDto
{
    public int CoverageGapAlertId { get; set; }
    public int CoverageAssignmentId { get; set; }
    public string AlertStatus { get; set; } = default!;
    public string AlertReason { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateOnly CoverageDate { get; set; }
    public string ShiftType { get; set; } = default!;
    public string SpecialtyName { get; set; } = default!;
}