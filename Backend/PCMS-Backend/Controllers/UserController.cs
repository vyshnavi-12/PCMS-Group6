using Microsoft.AspNetCore.Mvc;
using PCMS_Backend.DTOs;
using PCMS_Backend.Shared;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using PCMS_Backend.Interfaces.Services;

namespace PCMS_Backend.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto req)
    {
        var result = await _userService.RegisterAsync(req);
        return result.ToActionResult();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto req)
    {
        var result = await _userService.LoginAsync(req);

        if (!result.Success)
            return result.ToActionResult();

        var token = result.Token!;

        Response.Cookies.Append("token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true, 
            SameSite = SameSiteMode.Strict, 
            Expires = DateTime.UtcNow.AddMinutes(30)
        });

        return result.ToActionResult();
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("token");

        return Ok(new
        {
            success = true,
            message = "Logged out successfully"
        });
    }

    [HttpGet("checkAuth")]
    [Authorize]
    public async Task<IActionResult> CheckAuth()
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(claimValue, out int userId))
        {
            var result = await _userService.GetNameAsync(userId);
            return result.ToActionResult();
        }

        return Unauthorized("Invalid or missing session token.");
    }

}