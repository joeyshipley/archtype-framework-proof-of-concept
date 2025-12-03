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
    public string RenderPage() =>
        _renderer.Render(
            new Section()
                .Children(
                    new PageTitle("Login"),
                    new Section().Id("notifications"),
                    renderLoginFormComponent()
                )
        );

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
            .Id("login-form")
            .Children(
                new Form()
                    .Action("/interaction/login/authenticate")
                    .Swap(SwapStrategy.None)
                    .Children(
                        new Stack(For.Fields)
                            .Children(
                                new Field()
                                    .Label(new Label("Email").For("email"))
                                    .Input(new Input()
                                        .Name("email")
                                        .Type(InputType.Email)
                                        .Placeholder("Enter email")
                                        .Id("email")
                                    ),
                                new Field()
                                    .Label(new Label("Password").For("password"))
                                    .Input(new Input()
                                        .Name("password")
                                        .Type(InputType.Password)
                                        .Placeholder("Enter password")
                                        .Id("password")
                                    )
                            ),
                        new Button(Importance.Primary, "Login")
                            .Type(ButtonType.Submit)
                    )
            );
}
