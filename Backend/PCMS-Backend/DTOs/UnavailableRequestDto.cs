using System.ComponentModel.DataAnnotations;

namespace PCMS_Backend.DTOs;
public class UnavailableRequestDto
{
  [Required]
  public string Reason { get; set; } = string.Empty;
}
