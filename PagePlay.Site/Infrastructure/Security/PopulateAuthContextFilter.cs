namespace PagePlay.Site.Infrastructure.Security;

public class PopulateAuthContextFilter(IJwtTokenService _jwtTokenService) : IEndpointFilter
{
    public async ValueTask<object> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var authContext = context.HttpContext.RequestServices
            .GetRequiredService<LoggedInAuthContext>();

        var userId = _jwtTokenService.GetCurrentUserId();
        if (userId == null)
            return Results.Unauthorized();

        authContext.UserId = userId.Value;

        return await next(context);
    }
}
