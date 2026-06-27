public class DoctorScheduleDto
{
    public int CoverageAssignmentId { get; set; }

    public int CoverageScheduleId { get; set; }

    public DateOnly Date { get; set; }

    public DateOnly WeekStartDate { get; set; }

    public DateOnly WeekEndDate { get; set; }

    public string Shift { get; set; } = default!;
    public string Specialty { get; set; } = default!;
    public string Time { get; set; } = default!;
    public string Status { get; set; } = default!;
}