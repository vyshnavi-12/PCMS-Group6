using PCMS_Backend.Models;
using System.ComponentModel.DataAnnotations;

namespace PCMS_Backend.Models;

    public class Specialty
    {
        
        public int SpecialtyId { get; set; }

        public string SpecialtyName { get; set; } = default!;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<PhysicianSpecialtyMap> PhysicianSpecialtyMaps { get; set; } = default!;

        public ICollection<CoverageAssignment> CoverageAssignments { get; set; } = default!;
    }
