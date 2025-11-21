namespace PagePlay.Site.Application.Accounts.Register;

public class RegisterRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

public class RegisterResponse
{
    public string Message { get; set; }
}