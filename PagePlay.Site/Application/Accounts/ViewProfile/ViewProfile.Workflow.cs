using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Application.Accounts.ViewProfile;

public class ViewProfileWorkflow() : IWorkflow<ViewProfileRequest, ViewProfileResponse>
{
    public Task<IApplicationResult<ViewProfileResponse>> Perform(ViewProfileRequest request)
    {
        return Task.FromResult(response());
    }

    private IApplicationResult<ViewProfileResponse> response() =>
        ApplicationResult<ViewProfileResponse>.Succeed(
            new ViewProfileResponse { Message = "ViewProfile API Workflow is GO!" }
        );
}