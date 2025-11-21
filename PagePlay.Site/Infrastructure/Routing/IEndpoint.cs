namespace PagePlay.Site.Infrastructure.Routing;

public interface IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints);
}