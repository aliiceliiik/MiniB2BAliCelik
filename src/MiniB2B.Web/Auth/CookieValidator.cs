using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MiniB2B.Business.Services;

namespace MiniB2B.Web.Auth;

public static class CookieValidator
{
    private static readonly TimeSpan ValidationInterval = TimeSpan.FromMinutes(1);

    public static async Task ValidateAsync(CookieValidatePrincipalContext context)
    {
        if (context.Principal?.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
            return;

        var lastValidatedClaim = identity.FindFirst(UserClaimsFactory.LastValidatedClaim);

        if (DateTimeOffset.TryParse(lastValidatedClaim?.Value, CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind, out var lastValidated)
            && DateTimeOffset.UtcNow - lastValidated < ValidationInterval)
        {
            return;
        }

        var userIdValue = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = identity.FindFirst(ClaimTypes.Role)?.Value;
        var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();

        if (!int.TryParse(userIdValue, out var userId)
            || role is null
            || !await authService.IsSessionValidAsync(userId, role))
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return;
        }

        if (lastValidatedClaim is not null)
            identity.RemoveClaim(lastValidatedClaim);

        identity.AddClaim(new Claim(UserClaimsFactory.LastValidatedClaim,
            DateTimeOffset.UtcNow.ToString("O", CultureInfo.InvariantCulture)));

        context.ShouldRenew = true;
    }
}