using System.ComponentModel.DataAnnotations;

namespace Physician_On_Call_Schedule_Management_System.Models
{
    public class Specialties
    {
        [Key]
       public int SpecialtyId { get; set; }
        public string SpecialtyName { get; set; } = default!;
       public  bool IsActive { get; set; }
       public  DateTime CreatedAt { get; set; }

    }
}
