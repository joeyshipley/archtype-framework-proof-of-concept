using FluentValidation.Results;

namespace PagePlay.Site.Infrastructure.Core.Application;

public static class ValidationResultExtensions
{
	public static IEnumerable<ResponseErrorEntry> ToResponseErrors(this ValidationResult validationResult)
    {
        return validationResult.Errors.Select(e => new ResponseErrorEntry
        {
            Message = e.ErrorMessage, 
            Property = e.PropertyName.ToLowerFirstCharacter()
        });
    }
}