namespace PCMS_Backend.DTOs;
public class OpenAlertsResponseDto
{ 
    public int AlertId { get; set; }
    public DateOnly Date { get; set; }  
    public string Specialty { get; set; } = default!;

    public string Shift { get; set; } = default!;
    public string RequestedBy { get; set; } = default!;

    public string Status    { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

}
