using PagePlay.Site.Infrastructure.Core.Application;

namespace PagePlay.Site.Application.StyleTest.GetRandomNumber;

public class GetRandomNumberWorkflow
    : WorkflowBase<GetRandomNumberWorkflowRequest, GetRandomNumberWorkflowResponse>,
      IWorkflow<GetRandomNumberWorkflowRequest, GetRandomNumberWorkflowResponse>
{
    public Task<IApplicationResult<GetRandomNumberWorkflowResponse>> Perform(GetRandomNumberWorkflowRequest workflowRequest)
    {
        var randomNumber = generateRandomNumber();
        return Task.FromResult(Succeed(buildResponse(randomNumber)));
    }

    private int generateRandomNumber() =>
        Random.Shared.Next(1, 1000);

    private GetRandomNumberWorkflowResponse buildResponse(int number) =>
        new GetRandomNumberWorkflowResponse
        {
            Number = number
        };
}
