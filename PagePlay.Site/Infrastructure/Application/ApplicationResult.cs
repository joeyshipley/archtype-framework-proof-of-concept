using FluentValidation.Results;

namespace PagePlay.Site.Infrastructure.Application;

public interface IApplicationResult<out T>
{
    bool Success { get; }
    IEnumerable<ResponseErrorEntry>? Errors { get; }
    T? Model { get; }
}

public class ApplicationResult<T> : IApplicationResult<T>
{
    public bool Success { get; set; }
    public T? Model { get; set; }
    public IEnumerable<ResponseErrorEntry>? Errors { get; set; } = [];

    public static IApplicationResult<T> Succeed(T? model) => 
        new ApplicationResult<T> { Success = true, Model = model };

    public static IApplicationResult<T> Fail(ValidationResult validationResult) => 
        new ApplicationResult<T>
        {
            Success = false, 
            Errors = validationResult.ToResponseErrors()
        };
}