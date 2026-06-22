namespace PCMS_Backend.DTOs
{
    public class UncoveredSlotDto
    {

        public DateOnly Date { get; set; }

        public string ShiftType { get; set; } = string.Empty;

        public int SpecialtyId { get; set; }

    }
}
