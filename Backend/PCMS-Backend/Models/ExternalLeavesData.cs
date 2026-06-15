using System.ComponentModel.DataAnnotations;
namespace PCMS_Backend.Models;

public class ExternalLeavesData
{
    [Key]
    public int ExternalLeaveId { get; set; }

    public int PhysicianId { get; set; }

    public DateOnly LeaveStartDate { get; set; }

    public DateOnly LeaveEndDate { get; set; }

    public string LeaveReason { get; set; } = default!;

    public Physician Physician { get; set; } = default!;
}