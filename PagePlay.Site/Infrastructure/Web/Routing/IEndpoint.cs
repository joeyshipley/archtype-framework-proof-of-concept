namespace PagePlay.Site.Infrastructure.Web.Routing;

public interface IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints);
}