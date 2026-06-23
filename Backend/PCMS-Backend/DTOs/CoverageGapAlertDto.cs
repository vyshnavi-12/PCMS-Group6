namespace PCMS_Backend.DTOs;

public class CoverageGapAlertDto
{
    public int CoverageAssignmentId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string AlertReason { get; set; } = string.Empty;

    public string AlertStatus { get; set; } = string.Empty;
}