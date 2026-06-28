namespace PCMS_Backend.DTOs;

public class SupervisorSwapRequestDto
{
    public int SwapRequestId { get; set; }
    public string RequestedBy { get; set; } = default!;
    public string TargetPhysician { get; set; } = default!;
    public string Shift { get; set; } = default!;
    public string RequestedPhysicianDate { get; set; } = default!;
    public string TargetedPhysicianDate { get; set; } = default!;
    public string Status { get; set; } = default!;
}