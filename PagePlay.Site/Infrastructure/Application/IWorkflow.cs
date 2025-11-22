namespace PagePlay.Site.Infrastructure.Application;

public interface IWorkflow<in TRequest, TResponse>
    where TRequest : IRequest
    where TResponse : IResponse
{
    Task<IApplicationResult<TResponse>> Execute(TRequest request);
}

public interface IWorkflow<TResponse>
    where TResponse : IResponse
{
    Task<IApplicationResult<TResponse>> Execute();
}
