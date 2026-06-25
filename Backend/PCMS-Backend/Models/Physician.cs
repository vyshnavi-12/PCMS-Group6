using System.ComponentModel.DataAnnotations;

namespace PCMS_Backend.Models;

public class Physician
    {
        
        public int PhysicianId { get; set; }

        public int? UserId { get; set; }

        public string PhysicianCode { get; set; } = default!;

        public string MedicalLicenseNumber { get; set; } = default!;

        public string EmploymentStatus { get; set; } = default!;

        public DateTime JoinedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = default!;

        public ICollection<PhysicianSpecialtyMap> PhysicianSpecialtyMaps { get; set; } = default!;

        public ICollection<ExternalShiftsData> ExternalShiftsData { get; set; } = default!;

        public ICollection<ExternalLeavesData> ExternalLeavesData { get; set; } = default!;

        public ICollection<CoverageAssignment> CoverageAssignments { get; set; } = default!;

    public ICollection<CoverageGapAlert> CoverageGapAlert { get; set; } = default!;


    public ICollection<SwapRequest> RequestedSwapRequests { get; set; } = default!;

        public ICollection<SwapRequest> TargetSwapRequests { get; set; } = default!;
    }
