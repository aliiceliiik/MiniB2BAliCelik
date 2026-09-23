using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using MiniB2B.Entities.Dtos.Auth;
using System.Globalization;

namespace MiniB2B.Web.Auth;

public static class UserClaimsFactory
{
    public const string FullNameClaim = "FullName";
    public const string LastValidatedClaim = "LastValidatedUtc";

    public static ClaimsPrincipal Create(AuthenticatedUserDto user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(FullNameClaim, user.FullName),
            new(LastValidatedClaim, DateTimeOffset.UtcNow.ToString("O", CultureInfo.InvariantCulture))
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }
}