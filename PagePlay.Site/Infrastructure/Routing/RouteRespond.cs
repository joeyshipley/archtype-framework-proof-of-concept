using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Infrastructure.Routing;

public static class Respond
{
    public static IResult Ok<T>(IApplicationResult<T> result)
    {
        var errorResponse = DetermineError(result);
        return errorResponse ?? Results.Ok(result.Model);
    }
    
    private static IResult DetermineError<T>(IApplicationResult<T> result)
    {
        switch (result.ErrorType)
        {
            case ResultErrorType.ValidationError:
                //Telemetry.Warn("Validation Error: {ValidationError}", result.ValidationErrors);
                return Results.BadRequest(result.ValidationErrors ?? new List<ResponseErrorEntry>());
            case ResultErrorType.NotFound:
                //Telemetry.Warn("Not Found Error: {ErrorMessage}", result.ErrorMessage);
                return Results.NotFound(result.ErrorMessage);
            case ResultErrorType.AuthenticationError:
                //Telemetry.Error("Authentication Error: {ErrorMessage}", result.ErrorMessage);
                return Results.Unauthorized();
            case ResultErrorType.ForbiddenError:
                //Telemetry.Error("Forbidden Error: {ErrorMessage}", result.ErrorMessage);
                return Results.Forbid();
            case ResultErrorType.ConflictError:
                //Telemetry.Error("Conflict Error: {ErrorMessage}", result.ErrorMessage);
                return Results.Conflict(result.ErrorMessage);
            default:
                //Telemetry.Error("Conflict Error: {ErrorMessage}", result.ErrorMessage);
                return Results.Problem(
                    detail: $"Unhandled Error: {result.ErrorMessage}",
                    statusCode: StatusCodes.Status500InternalServerError
                );
        }
    }
}