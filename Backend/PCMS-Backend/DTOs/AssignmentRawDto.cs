namespace PCMS_Backend.DTOs
{

    public class AssignmentRawDto
    {
        public int PhysicianId { get; set; }
        public int SpecialtyId { get; set; }

        public DateOnly Date { get; set; }
        public string ShiftType { get; set; } = default!;
    }

}
