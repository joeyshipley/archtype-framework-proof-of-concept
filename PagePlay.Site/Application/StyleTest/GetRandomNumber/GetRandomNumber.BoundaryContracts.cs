using PagePlay.Site.Infrastructure.Core.Application;

namespace PagePlay.Site.Application.StyleTest.GetRandomNumber;

public class GetRandomNumberResponse : IPerformerResponse
{
    public required int Number { get; set; }
}

public class GetRandomNumberRequest : IPerformerRequest
{
    public long Id { get; set; }
}
