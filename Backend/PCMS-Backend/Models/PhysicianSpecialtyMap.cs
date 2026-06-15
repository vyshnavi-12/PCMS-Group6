using System;
using System.ComponentModel.DataAnnotations;

namespace Physician_On_Call_Schedule_Management_System.Models
{
    public class PhysicianSpecialtyMap
    {
        [Key]
        public int PhysicianSpecialtyMapId { get; set; }

        public int PhysicianId { get; set; }

        public int SpecialtyId { get; set; }

        public bool IsPrimarySpecialty { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
