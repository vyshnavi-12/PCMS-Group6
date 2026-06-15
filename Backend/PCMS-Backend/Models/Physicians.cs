using System;
using System.ComponentModel.DataAnnotations;

namespace Physician_On_Call_Schedule_Management_System.Models
{
    public class Physicians
    {
        [Key]
        public int PhysicianId { get; set; }

        public int UserId { get; set; }

        public string PhysicianCode { get; set; } = default!;

        public string MedicalLicenseNumber { get; set; } = default!;

        public string EmploymentStatus { get; set; } = default!;

        public DateTime JoinedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
