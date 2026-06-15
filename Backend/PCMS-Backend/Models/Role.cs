using System.ComponentModel.DataAnnotations;

namespace  PCMS_Backend.Models;

public class Role
{
    [Key]
    public int RoleId { get; set; }

    [StringLength(50)]
    public string RoleName { get; set; } = default!;

    // Navigation Properties

    public ICollection<User> Users { get; set; }
        = new List<User>();
}