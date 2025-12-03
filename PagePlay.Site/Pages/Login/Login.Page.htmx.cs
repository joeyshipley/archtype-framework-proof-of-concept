using PagePlay.Site.Infrastructure.UI.Rendering;
using PagePlay.Site.Infrastructure.UI.Vocabulary;

namespace PagePlay.Site.Pages.Login;

public interface ILoginPageView
{
    string RenderPage();
    string RenderLoginForm();
    string RenderError(string error);
    string RenderSuccess(string message);
}

public class LoginPage(IHtmlRenderer _renderer) : ILoginPageView
{
    public string RenderPage()
    {
        var page = new Section();
        page.Add(new PageTitle("Login"));
        page.Add(new Section { Id = "notifications" });
        page.Add(renderLoginFormComponent());

        return _renderer.Render(page);
    }

    public string RenderLoginForm()
    {
        var form = renderLoginFormComponent();
        return _renderer.Render(form);
    }

    public string RenderError(string error)
    {
        var alert = new Alert(error, AlertTone.Critical);
        return _renderer.Render(alert);
    }

    public string RenderSuccess(string message)
    {
        var alert = new Alert(message, AlertTone.Positive);
        return _renderer.Render(alert);
    }

    private Section renderLoginFormComponent()
    {
        var section = new Section { Id = "login-form" };

        var form = new Form
        {
            Action = "/interaction/login/authenticate",
            Swap = SwapStrategy.None
        };

        var emailField = new Field
        {
            Label = new Label("Email") { For = "email" },
            Input = new Input
            {
                Name = "email",
                Type = InputType.Email,
                Placeholder = "Enter email",
                Id = "email"
            }
        };

        var passwordField = new Field
        {
            Label = new Label("Password") { For = "password" },
            Input = new Input
            {
                Name = "password",
                Type = InputType.Password,
                Placeholder = "Enter password",
                Id = "password"
            }
        };

        var fieldsStack = new Stack(For.Fields);
        fieldsStack.Add(emailField);
        fieldsStack.Add(passwordField);

        var loginButton = new Button(Importance.Primary, "Login") { Type = ButtonType.Submit };

        form.Add(fieldsStack);
        form.Add(loginButton);

        section.Add(form);

        return section;
    }
}
