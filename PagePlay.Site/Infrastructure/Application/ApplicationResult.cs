using FluentValidation.Results;

namespace PagePlay.Site.Infrastructure.Application;

public enum ResultErrorType
{
    ValidationError,
    AuthenticationError,
    NotFound,
    ForbiddenError,
    ConflictError
}

public interface IApplicationResult<out T>
{
    ResultErrorType? ErrorType { get; }
    IEnumerable<ResponseErrorEntry>? ValidationErrors { get; }
    string? ErrorMessage { get; }
    T? Model { get; }
}

public class ApplicationResult<T> : IApplicationResult<T>
{
    public T? Model { get; set; }
    public ResultErrorType? ErrorType { get; set; }
    public string? ErrorMessage { get; set; }
    public IEnumerable<ResponseErrorEntry>? ValidationErrors { get; set; }

    public static IApplicationResult<T> Succeed(T? model) => new ApplicationResult<T> { Model = model };

    public static IApplicationResult<T> Fail(ValidationResult validationResult) => new ApplicationResult<T>
    {
        ErrorType = ResultErrorType.ValidationError, ValidationErrors = validationResult.ToResponseErrors()
    };

    public static IApplicationResult<T> FailAuthentication(string? message = null) =>
        new ApplicationResult<T> { ErrorType = ResultErrorType.AuthenticationError, ErrorMessage = message };

    public static IApplicationResult<T> FailNotFound(string? message = null) =>
        new ApplicationResult<T> { ErrorType = ResultErrorType.NotFound, ErrorMessage = message };

    public static IApplicationResult<T> FailForbidden(string? message = null) =>
        new ApplicationResult<T> { ErrorType = ResultErrorType.ForbiddenError, ErrorMessage = message };

    public static IApplicationResult<T> FailConflict(string? message = null) =>
        new ApplicationResult<T> { ErrorType = ResultErrorType.ConflictError, ErrorMessage = message };

    public static IApplicationResult<T> Transform<TSource>(IApplicationResult<TSource> source)
    {
        if (source.Model is not null)
            throw new ArgumentException("Source model must be null to transform to a different type.");

        return new ApplicationResult<T>
        {
            ErrorType = source.ErrorType,
            ErrorMessage = source.ErrorMessage,
            ValidationErrors = source.ValidationErrors
        };
    }
}