namespace PCMS_Backend.DTOs;

public class SupervisorSwapRequestDto
{
    public int SwapRequestId { get; set; }
    public string RequestedBy { get; set; } = default!;
    public string TargetPhysician { get; set; } = default!;
    public string Shift { get; set; } = default!;
    public string Date { get; set; } = default!;
    public string Status { get; set; } = default!;
}