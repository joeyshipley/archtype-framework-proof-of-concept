namespace PagePlay.Site.Infrastructure.Security;

public class PopulateAuthContextFilter(IUserIdentityService _userIdentityService) : IEndpointFilter
{
    public async ValueTask<object> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var authContext = context.HttpContext.RequestServices
            .GetRequiredService<CurrentUserContext>();

        var userId = _userIdentityService.GetCurrentUserId();
        if (userId == null)
            return Results.Unauthorized();

        authContext.UserId = userId.Value;

        return await next(context);
    }
}
