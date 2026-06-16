using System.ComponentModel.DataAnnotations;

namespace  PCMS_Backend.Models;

public class Role
{
    
    public int RoleId { get; set; }

    
    public string RoleName { get; set; } = default!;



    

    public ICollection<User> Users { get; set; }
        = default!;
}