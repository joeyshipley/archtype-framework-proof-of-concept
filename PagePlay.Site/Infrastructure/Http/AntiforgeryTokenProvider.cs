using Microsoft.AspNetCore.Antiforgery;

namespace PagePlay.Site.Infrastructure.Http;

public interface IAntiforgeryTokenProvider
{
    string GetRequestToken();
}

public class AntiforgeryTokenProvider(
    IAntiforgery _antiforgery,
    IHttpContextAccessor _httpContextAccessor
) : IAntiforgeryTokenProvider
{
    public string GetRequestToken()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
            throw new InvalidOperationException("HttpContext is not available");

        var tokens = _antiforgery.GetAndStoreTokens(context);
        return tokens.RequestToken;
    }
}
