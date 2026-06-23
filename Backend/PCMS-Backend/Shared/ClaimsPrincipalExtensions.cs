using System.Security.Claims;

namespace PCMS_Backend.Shared;

public static class ClaimsPrincipalExtensions
{
    public static int? GetCurrentUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(claim, out int id)) return id;
        return null;
    }
}