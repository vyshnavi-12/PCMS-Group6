namespace PCMS_Backend.DTOs;

public class MySwapRequestDto
{
    public int SwapRequestId { get; set; }

    public string CurrentDate { get; set; } = default!;
    public string RequestedDate { get; set; } = default!;

    public string Shift { get; set; } = default!;

    public string RequestedWith { get; set; } = default!;

    public string Status { get; set; } = default!;

    public string RequestedOn { get; set; } = default!;

    public string Reason { get; set; } = default!;
}