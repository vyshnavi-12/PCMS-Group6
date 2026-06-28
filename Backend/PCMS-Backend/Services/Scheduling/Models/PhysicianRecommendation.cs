namespace PCMS_Backend.Services.Scheduling.Models;

public class PhysicianRecommendation
{
    public int PhysicianId { get; set; }

    public string PhysicianName { get; set; } = default!;

    public int Score { get; set; }

    public bool IsPrimarySpecialty { get; set; }



}