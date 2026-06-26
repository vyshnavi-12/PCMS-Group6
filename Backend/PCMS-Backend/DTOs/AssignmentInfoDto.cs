namespace PCMS_Backend.DTOs
{

    public class AssignmentInfoDto
    {
        public DateOnly Date { get; set; }
        public string ShiftType { get; set; } = default!;
    }

}
