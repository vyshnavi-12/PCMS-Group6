using PCMS_Backend.Models;
using PCMS_Backend.DTOs;
using PCMS_Backend.Shared;
using Microsoft.AspNetCore.Identity;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Interfaces.Repositories;

namespace PCMS_Backend.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IPhysicianRepository _physicianRepo;
    private readonly IRoleRepository _roleRepo;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly AuthService _authService;

    public UserService(
        IUserRepository userRepo,
        IPhysicianRepository physicianRepo,
        IRoleRepository roleRepo,
        AuthService authService)
    {
        _userRepo = userRepo;
        _physicianRepo = physicianRepo;
        _roleRepo = roleRepo;
        _passwordHasher = new PasswordHasher<User>();
        _authService = authService;
    }

    public async Task<Result> RegisterAsync(RegisterRequestDto req)
    {
        string email = req.EmailAddress.ToLower();
        string employeeCode = req.EmployeeCode.Trim();

        if (await _userRepo.ExistsByEmployeeCodeAsync(employeeCode))
            return Result.Conflict("Employee Code already exists.");

        if (await _userRepo.ExistsByEmailAsync(email))
            return Result.Conflict("Email address is already registered.");

        var role = await _roleRepo.GetByIdAsync(req.RoleId);
        if (role == null)
            return Result.NotFound("The specified RoleId does not exist.");

        Physician? existingPhysician = null;

        if (role.RoleName == "Physician")
        {
            if (string.IsNullOrWhiteSpace(req.PhysicianCode))
                return Result.BadRequest("Physician Code is required to register a Physician account.");

            existingPhysician = await _physicianRepo.GetByCodeAsync(req.PhysicianCode);

            if (existingPhysician == null)
                return Result.NotFound("Physician record not found. Please verify your Physician Code.");

            if (existingPhysician.UserId != null)
                return Result.Conflict("This Physician Code has already been registered to an account.");
        }

        var user = new User
        {
            EmployeeCode = employeeCode,
            FullName = req.FullName,
            EmailAddress = email,
            PhoneNumber = req.PhoneNumber,
            RoleId = req.RoleId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, req.Password);

        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();

        if (role.RoleName == "Physician" && existingPhysician != null)
        {
            existingPhysician.UserId = user.UserId;
            _physicianRepo.Update(existingPhysician);
            await _userRepo.SaveChangesAsync();
        }

        return Result.Created("User registered successfully.");
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto req)
    {
        string email = req.Email.ToLower();

        var user = await _userRepo.GetUserByEmailWithRoleAsync(email);

        if (user == null)
            return Result<LoginResponseDto>.NotFound("User not found");

        if (!user.IsActive)
            return Result<LoginResponseDto>.Forbidden("User account is currently deactivated.");

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            req.Password
        );

        if (result == PasswordVerificationResult.Failed)
        {
            return Result<LoginResponseDto>.Unauthorized("Login failed due to bad credentials.");
        }

        string token = _authService.GenerateToken(user, user.Role.RoleName);

        var userResponse = new LoginResponseDto
        {
            FullName = user.FullName,
            Role = user.Role.RoleName
        };

        return Result<LoginResponseDto>.Ok(userResponse, "Login successful", token);
    }

    public async Task<Result<MeResponseDto>> GetNameAsync(int userId)
    {
        var user = await _userRepo.GetUserByIdWithRoleAsync(userId);

        if (user == null)
            return Result<MeResponseDto>.NotFound("User profile not found.");

        var userResponse = new MeResponseDto
        {
            FullName = user.FullName,
            Role = user.Role.RoleName
        };

        return Result<MeResponseDto>.Ok(userResponse, "User is authenticated");
    }
}