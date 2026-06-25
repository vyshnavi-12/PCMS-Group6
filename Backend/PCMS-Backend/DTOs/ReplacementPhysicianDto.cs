namespace PCMS_Backend.DTOs;
public class ReplacementPhysicianDto

{
    public int PhysicianId { get; set; }
    public string PhysicianName { get; set; } = default!;
    public bool IsRecommended { get; set; }
}