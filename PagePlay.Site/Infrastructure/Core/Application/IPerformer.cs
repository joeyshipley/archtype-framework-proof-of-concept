namespace PagePlay.Site.Infrastructure.Core.Application;

public interface IPerformer<in TRequest, TResponse>
    where TRequest : IPerformerRequest
    where TResponse : IPerformerResponse
{
    Task<IApplicationResult<TResponse>> Perform(TRequest request);
}
