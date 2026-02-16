using System.Security.Claims;

namespace StancaBlogApi.Infrastructure.Security;

public static class ClaimsPrincipalExtensions
{
    public static bool TryGetUserId(this ClaimsPrincipal principal, out int userId)
    {
        userId = 0;
        var userIdValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return userIdValue is not null && int.TryParse(userIdValue, out userId);
    }

    public static string GetUserNameOrEmpty(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
    }
}
