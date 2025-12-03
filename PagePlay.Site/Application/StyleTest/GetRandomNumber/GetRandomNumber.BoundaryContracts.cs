using PagePlay.Site.Infrastructure.Core.Application;

namespace PagePlay.Site.Application.StyleTest.GetRandomNumber;

public class GetRandomNumberWorkflowResponse : IWorkflowResponse
{
    public required int Number { get; set; }
}

public class GetRandomNumberWorkflowRequest : IWorkflowRequest
{
    public long Id { get; set; }
}
