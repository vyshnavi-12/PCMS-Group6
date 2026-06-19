namespace PCMS_Backend.DTOs;

public class MeResponseDto
{
    public string FullName { get; set; } = default!;
    public string Role { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string EmployeeCode { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? SpecialtyName { get; set; }
}