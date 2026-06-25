namespace PCMS_Backend.DTOs
{
    public class TopPhysicianPerSpecialtyDto
    {
        public int PhysicianId { get; set; }
        public string PhysicianName { get; set; } = default!;

        public int SpecialtyId { get; set; }
        public string SpecialtyName { get; set; }=default!;

        public int TotalAssignments { get; set; }

        public List<AssignmentInfoDto> Assignments { get; set; } = new();
    }
}
