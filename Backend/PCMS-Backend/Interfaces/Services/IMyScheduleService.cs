using PCMS_Backend.DTOs;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Interfaces.Services;

public interface IMyScheduleService
{
    Task<Result<IReadOnlyList<DoctorScheduleDto>>> GetMyScheduleAsync(int userId);
}