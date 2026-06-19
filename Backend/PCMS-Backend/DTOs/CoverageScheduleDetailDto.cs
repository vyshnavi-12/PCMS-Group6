namespace PCMS_Backend.DTOs
{
    public class CoverageScheduleDetailDto
    {
        public int CoverageScheduleId { get; set; }

        public string ScheduleName { get; set; } = default!;

        public DateOnly WeekStartDate { get; set; }

        public DateOnly WeekEndDate { get; set; }

        public string Status { get; set; } = default!;

        public List<CoverageAssignmentDto> Assignments { get; set; }
            = new();
    }
}
