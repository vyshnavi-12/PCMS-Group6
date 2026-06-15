using System.ComponentModel.DataAnnotations;

namespace PCMS_Backend.Models;

public class PhysicianSpecialtyMap
{
    [Key]
    public int PhysicianSpecialtyMapId { get; set; }

    public int PhysicianId { get; set; }

    public int SpecialtyId { get; set; }

    public bool IsPrimarySpecialty { get; set; }

    public DateTime CreatedAt { get; set; }

    public Physician Physician { get; set; } = default!;

    public Specialty Specialty { get; set; } = default!;
}