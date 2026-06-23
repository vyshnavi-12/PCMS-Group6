namespace PCMS_Backend.DTOs;

public class RequestToMeDto
{
    public int SwapRequestId { get; set; }

    public string Date { get; set; } = default!;

    public string Shift { get; set; } = default!;

    public string RequestedBy { get; set; } = default!;

    public string Reason { get; set; } = default!;

    public string RequestedOn { get; set; } = default!;

    public string Status { get; set; } = default!;
}