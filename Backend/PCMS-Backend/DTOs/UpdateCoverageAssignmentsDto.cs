namespace PCMS_Backend.DTOs
{
    public class UpdateCoverageAssignmentsDto
    {
        public List<AssignmentUpdateDto> Assignments { get; set; }
            = new();
    }

    public class AssignmentUpdateDto
    {
        public int CoverageAssignmentId { get; set; }

        public int PhysicianId { get; set; }
    }
}
