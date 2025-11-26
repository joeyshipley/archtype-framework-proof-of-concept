using FluentValidation.Results;

namespace PagePlay.Site.Infrastructure.Core.Application;

public abstract class WorkflowBase<TRequest, TResponse>
    where TRequest : IWorkflowRequest
    where TResponse : IWorkflowResponse
{
    protected IApplicationResult<TResponse> Fail(ValidationResult validationResult) =>
        ApplicationResult<TResponse>.Fail(validationResult);

    protected IApplicationResult<TResponse> Fail(string errorMessage) =>
        ApplicationResult<TResponse>.Fail(errorMessage);

    protected IApplicationResult<TResponse> Succeed(TResponse response) =>
        ApplicationResult<TResponse>.Succeed(response);
}
