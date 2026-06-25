namespace PCMS_Backend.DTOs
{

    public class TopPhysicianRawDto
    {
        public int PhysicianId { get; set; }
        public string PhysicianName { get; set; }=default!;

        public int SpecialtyId { get; set; }
        public string SpecialtyName { get; set; }=default!;

        public int TotalAssignments { get; set; }
    }

}
