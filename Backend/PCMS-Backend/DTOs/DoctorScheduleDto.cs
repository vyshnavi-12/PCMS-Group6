namespace PCMS_Backend.DTOs;

public class DoctorScheduleDto
{
    public DateOnly Date { get; set; }

    public string Shift { get; set; } = default!;

    public string Specialty { get; set; } = default!;

    public string Time { get; set; } = default!;

    public string Status { get; set; } = default!;
}