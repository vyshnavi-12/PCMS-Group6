using System.ComponentModel.DataAnnotations;

namespace PCMS_Backend.DTOs;

public class RegisterRequestDto
{
    [Required]
    public string EmployeeCode { get; set; } = default!;

    [Required]
    public string FullName { get; set; } = default!;

    [Required, EmailAddress]
    public string EmailAddress { get; set; } = default!;

    [Required]
    public string PhoneNumber { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;

    [Required]
    public int RoleId { get; set; }
    public string? PhysicianCode { get; set; }
}