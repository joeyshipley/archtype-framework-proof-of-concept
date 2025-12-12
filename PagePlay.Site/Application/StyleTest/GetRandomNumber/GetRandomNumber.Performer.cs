using PagePlay.Site.Infrastructure.Core.Application;

namespace PagePlay.Site.Application.StyleTest.GetRandomNumber;

public class GetRandomNumberPerformer
    : PerformerBase<GetRandomNumberRequest, GetRandomNumberResponse>,
      IPerformer<GetRandomNumberRequest, GetRandomNumberResponse>
{
    public Task<IApplicationResult<GetRandomNumberResponse>> Perform(GetRandomNumberRequest request)
    {
        var randomNumber = generateRandomNumber();
        return Task.FromResult(Succeed(buildResponse(randomNumber)));
    }

    private int generateRandomNumber() =>
        Random.Shared.Next(1, 1000);

    private GetRandomNumberResponse buildResponse(int number) =>
        new GetRandomNumberResponse
        {
            Number = number
        };
}
