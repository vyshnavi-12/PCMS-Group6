namespace PCMS_Backend.DTOs;

public class CoverageScheduleGenerateResponseDto
{
    public int ScheduleId { get; set; }

    public int TotalSlots { get; set; }

    public int AssignedSlots { get; set; }

    public double CoveragePercentage { get; set; }

    public List<UncoveredSlotDto> UncoveredSlots { get; set; } = new();
}