namespace PCMS_Backend.DTOs;

public class UnavailableRequestsPerSpecialtyDto
{
    public string SpecialtyName { get; set; } = string.Empty;
    public int RequestCount { get; set; }
}