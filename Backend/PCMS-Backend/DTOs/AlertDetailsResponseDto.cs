namespace PCMS_Backend.DTOs;

public class AlertDetailsResponseDto
{
    public string Reason { get; set; } = default!;
    public List<ReplacementPhysicianDto> Replacements { get; set; } = new();
}