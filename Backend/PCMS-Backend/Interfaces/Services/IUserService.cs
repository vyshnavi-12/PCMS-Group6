using PCMS_Backend.DTOs;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Interfaces.Services;

public interface IUserService
{
    Task<Result> RegisterAsync(RegisterRequestDto req);
    Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto req);
    Task<Result<MeResponseDto>> GetMeAsync(int userId);
}