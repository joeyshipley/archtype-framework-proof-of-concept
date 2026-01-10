using PagePlay.Site.Infrastructure.UI.Rendering;
using PagePlay.Site.Infrastructure.UI.Vocabulary;
using PagePlay.Site.Infrastructure.Web.Components;

namespace PagePlay.Site.Pages.Login;

public interface ILoginPageView : IView
{
    string RenderLoginForm();
    string RenderErrorNotification(string error);
    string RenderSuccessNotification(string message);
}

public class LoginPage(IHtmlRenderer _renderer) : ILoginPageView
{
    public string ViewId => "login-page";

    public DataDependencies Dependencies => DataDependencies.None;

    public string Render(IDataContext data) =>
        _renderer.Render(
            new Page(
                new PageTitle("Login"),
                new Section().Id("notifications"),
                new Grid(For.Sections, Columns.Three,
                    renderLoginFormComponent(),
                    new Section(
                        new Text("Welcome back! Sign in to access your todos and keep track of what needs to be done.")
                    ),
                    new Section()
                )
            ).Id(ViewId)
        );

    public string RenderLoginForm()
    {
        var form = renderLoginFormComponent();
        return _renderer.Render(form);
    }

    public string RenderErrorNotification(string error) =>
        _renderer.Render(
            new Section()
                .Id("notifications")
                .Children(
                    new Alert(error, AlertTone.Critical)
                )
        );

    public string RenderSuccessNotification(string message) =>
        _renderer.Render(
            new Section()
                .Id("notifications")
                .Children(
                    new Alert(message, AlertTone.Positive)
                )
        );

    private Section renderLoginFormComponent() =>
        new Section()
            .Id("login-form")
            .Children(
                new Stack(For.Sections)
                    .Children(
                        new Form()
                            .Action("/interaction/login/authenticate")
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
                                            ),
                                        new Button(Importance.Primary, "Login")
                                            .Type(ButtonType.Submit)
                                    )
                            ),
                        new Stack(For.Fields)
                            .Children(
                                new Text("Need an account?"),
                                new Link("Register", "/register")
                                    .Style(LinkStyle.ButtonSecondary)
                            )
                    )
            );
}
