using PCMS_Backend.DTOs;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Interfaces.Services;

public interface ISupervisorService
{
    Task<Result<List<OpenGapAlertDto>>> GetOpenGapAlertsAsync();
}