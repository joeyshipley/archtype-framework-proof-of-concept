namespace PagePlay.Site.Infrastructure.Http;

public interface ICookieManager
{
    void SetAuthCookie(string token);
}

public interface IResponseManager
{
    void SetRedirectHeader(string url);
}

public class CookieManager(IHttpContextAccessor _httpContextAccessor) : ICookieManager
{
    public void SetAuthCookie(string token)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
            throw new InvalidOperationException("HttpContext is not available");

        context.Response.Cookies.Append("auth_token", token, new CookieOptions
        {
            HttpOnly = true,                        // Prevents JavaScript access (XSS protection)
            Secure = true,                          // Only sent over HTTPS
            SameSite = SameSiteMode.Strict,         // CSRF protection
            MaxAge = TimeSpan.FromMinutes(60),      // Match JWT expiration
            Path = "/"
        });
    }
}

public class ResponseManager(IHttpContextAccessor _httpContextAccessor) : IResponseManager
{
    public void SetRedirectHeader(string url)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
            throw new InvalidOperationException("HttpContext is not available");

        context.Response.Headers.Append("HX-Redirect", url);
    }
}
