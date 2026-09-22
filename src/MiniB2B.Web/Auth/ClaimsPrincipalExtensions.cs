using System.Security.Claims;

namespace MiniB2B.Web.Auth;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(value, out var id)
            ? id
            : throw new InvalidOperationException("Kullanıcı kimliği bulunamadı.");
    }

    public static string GetFullName(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(UserClaimsFactory.FullNameClaim)
            ?? principal.Identity?.Name
            ?? string.Empty;
    }
}