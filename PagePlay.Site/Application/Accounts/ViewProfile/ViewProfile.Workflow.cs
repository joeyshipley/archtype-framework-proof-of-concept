using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Application.Accounts.ViewProfile;

public interface IViewProfileWorkflow
{
    Task<IApplicationResult<ViewProfileResponse>> ViewProfile(ViewProfileRequest request);
}

public class ViewProfileWorkflow() : IViewProfileWorkflow
{
    public Task<IApplicationResult<ViewProfileResponse>> ViewProfile(ViewProfileRequest request)
    {
        var response = new ViewProfileResponse { Message = "ViewProfile API Workflow is GO!" };
        return Task.FromResult(ApplicationResult<ViewProfileResponse>.Succeed(response));
    }
}