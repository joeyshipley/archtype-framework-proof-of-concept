using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PagePlay.Site.Infrastructure.Core.Application;

namespace PagePlay.Site.Infrastructure.Security;

public interface IJwtTokenService
{
    string GenerateToken(TokenClaims claims);
    TokenClaims ValidateToken(string token);
    long? GetCurrentUserId();
}

public class TokenClaims
{
    public long UserId { get; set; }
}

public class JwtTokenService(
    ISettingsProvider _settings,
    IHttpContextAccessor _httpContextAccessor
) : IJwtTokenService
{
    public string GenerateToken(TokenClaims tokenClaims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Security.Jwt.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, tokenClaims.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Security.Jwt.Issuer,
            audience: _settings.Security.Jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_settings.Security.Jwt.ExpirationMinutes)),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public TokenClaims ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Security.Jwt.SecretKey));

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _settings.Security.Jwt.Issuer,
                ValidAudience = _settings.Security.Jwt.Audience,
                IssuerSigningKey = securityKey,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return mapClaimsToTokenClaims(principal);
        }
        catch
        {
            return null;
        }
    }

    public long? GetCurrentUserId()
    {
        var principal = _httpContextAccessor.HttpContext?.User;
        if (principal == null)
            return null;

        var tokenClaims = mapClaimsToTokenClaims(principal);
        return tokenClaims?.UserId;
    }

    private TokenClaims mapClaimsToTokenClaims(ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ?? principal.FindFirst(JwtRegisteredClaimNames.Sub);

        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            return null;

        return new TokenClaims { UserId = userId };
    }
}
