namespace PagePlay.Site.Infrastructure.Application;

public interface IWorkflow<in TRequest, TResponse>
    where TRequest : IRequest
    where TResponse : IResponse
{
    Task<IApplicationResult<TResponse>> Perform(TRequest request);
}
