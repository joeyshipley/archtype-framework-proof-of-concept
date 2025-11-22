using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Application.Accounts.Login;

public class LoginResponse : IResponse
{
    public string Token { get; set; }
}

public class LoginRequest : IRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}