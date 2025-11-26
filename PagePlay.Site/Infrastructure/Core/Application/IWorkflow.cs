namespace PagePlay.Site.Infrastructure.Core.Application;

public interface IWorkflow<in TRequest, TResponse>
    where TRequest : IWorkflowRequest
    where TResponse : IWorkflowResponse
{
    Task<IApplicationResult<TResponse>> Perform(TRequest request);
}
