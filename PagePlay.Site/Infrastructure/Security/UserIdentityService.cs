using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PagePlay.Site.Infrastructure.Security;

/// <summary>
/// Service responsible for extracting user identity information from JWT claims.
/// Provides a single source of truth for accessing user data from ClaimsPrincipal.
/// </summary>
public interface IUserIdentityService
{
    /// <summary>
    /// Gets the current authenticated user's ID from HttpContext.
    /// Returns null if no user is authenticated or user ID cannot be parsed.
    /// </summary>
    long? GetCurrentUserId();

    /// <summary>
    /// Extracts the user ID from a ClaimsPrincipal.
    /// Returns null if claim is missing or cannot be parsed to a long.
    /// </summary>
    long? GetUserId(ClaimsPrincipal principal);

    /// <summary>
    /// Extracts the user ID as a string from a ClaimsPrincipal.
    /// Returns null if claim is missing or cannot be parsed.
    /// </summary>
    string? GetUserIdString(ClaimsPrincipal principal);

    /// <summary>
    /// Checks if the principal represents an authenticated user.
    /// </summary>
    bool IsAuthenticated(ClaimsPrincipal principal);
}

public class UserIdentityService(IHttpContextAccessor _httpContextAccessor) : IUserIdentityService
{
    public long? GetCurrentUserId()
    {
        var principal = _httpContextAccessor.HttpContext?.User;
        return principal != null ? GetUserId(principal) : null;
    }

    public long? GetUserId(ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)
            ?? principal.FindFirst(JwtRegisteredClaimNames.Sub);

        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
            return userId;

        return null;
    }

    public string? GetUserIdString(ClaimsPrincipal principal)
    {
        return GetUserId(principal)?.ToString();
    }

    public bool IsAuthenticated(ClaimsPrincipal principal)
    {
        return principal.Identity?.IsAuthenticated == true;
    }
}
