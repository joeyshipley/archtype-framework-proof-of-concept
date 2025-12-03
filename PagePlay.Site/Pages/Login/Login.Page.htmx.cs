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

    private Section renderLoginFormComponent() =>
        new Section()
            .WithId("login-form")
            .WithChildren(
                new Form()
                    .WithAction("/interaction/login/authenticate")
                    .WithSwap(SwapStrategy.None)
                    .WithChildren(
                        new Stack(For.Fields)
                            .WithChildren(
                                new Field()
                                    .WithLabel(new Label("Email").WithFor("email"))
                                    .WithInput(new Input()
                                        .WithName("email")
                                        .WithType(InputType.Email)
                                        .WithPlaceholder("Enter email")
                                        .WithId("email")
                                    ),
                                new Field()
                                    .WithLabel(new Label("Password").WithFor("password"))
                                    .WithInput(new Input()
                                        .WithName("password")
                                        .WithType(InputType.Password)
                                        .WithPlaceholder("Enter password")
                                        .WithId("password")
                                    )
                            ),
                        new Button(Importance.Primary, "Login")
                            .WithType(ButtonType.Submit)
                    )
            );
}
