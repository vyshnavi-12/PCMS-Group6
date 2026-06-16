using System.ComponentModel.DataAnnotations;
namespace PCMS_Backend.Models;

public class ExternalShiftsData
{
    
    public int ExternalShiftId { get; set; }

    public int PhysicianId { get; set; }

    public DateOnly ShiftDate { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public Physician Physician { get; set; } = default!;
}