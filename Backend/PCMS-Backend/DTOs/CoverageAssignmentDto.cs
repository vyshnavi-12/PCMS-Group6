namespace PCMS_Backend.DTOs
{
    public class CoverageAssignmentDto
    {
        public int CoverageAssignmentId { get; set; }

        public DateOnly CoverageDate { get; set; }

        public string SpecialtyName { get; set; } = default!;

        public string PhysicianName { get; set; } = default!;

        public string ShiftType { get; set; } = default!;

        public string AssignmentStatus { get; set; } = default!;
    }
}
