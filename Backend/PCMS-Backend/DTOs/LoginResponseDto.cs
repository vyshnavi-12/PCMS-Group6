namespace PCMS_Backend.DTOs;

public class LoginResponseDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = default!;
    public string Role { get; set; } = default!;
}