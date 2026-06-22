namespace PCMS_Backend.Models.Scheduling;

public class Slot
{
    public int Index { get; set; }
    public DateOnly Date { get; set; }
    public string ShiftType { get; set; } = string.Empty;
}