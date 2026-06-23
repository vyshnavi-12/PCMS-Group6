namespace PCMS_Backend.DTOs
{
    public class PhysicianWorkloadDto
    {
        public int PhysicianId { get; set; }
        public DateTime JoinDate { get; set; }
        public int MorningShiftCount { get; set; }
        public int NightShiftCount { get; set; }
    }

}