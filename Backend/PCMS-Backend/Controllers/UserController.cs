using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Shared;
using System.Security.Claims;

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
            SameSite = SameSiteMode.None,
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

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {

        if (User.GetCurrentUserId() is not int validUserId) return Unauthorized("Invalid or missing session token.");        
        var result = await _userService.GetMeAsync(validUserId);
        return result.ToActionResult();
    }


}