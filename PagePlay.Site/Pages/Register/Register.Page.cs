using PagePlay.Site.Infrastructure.UI.Rendering;
using PagePlay.Site.Infrastructure.UI.Vocabulary;
using PagePlay.Site.Infrastructure.Web.Components;

namespace PagePlay.Site.Pages.Register;

public interface IRegisterPageView : IView
{
    string RenderRegisterForm();
    string RenderErrorNotification(string error);
    string RenderSuccessNotification(string message);
}

public class RegisterPage(IHtmlRenderer _renderer) : IRegisterPageView
{
    public string ViewId => "register-page";

    public DataDependencies Dependencies => DataDependencies.None;

    public string Render(IDataContext data) =>
        _renderer.Render(
            new Page(
                new PageTitle("Create Account"),
                new Section().Id("notifications"),
                new Grid(For.Sections, Columns.Three,
                    renderRegisterFormComponent(),
                    new Section(
                        new Text("Join PagePlay to start organizing your tasks. Create an account to get started with your personal todo list.")
                    ),
                    new Section()
                )
            ).Id(ViewId)
        );

    public string RenderRegisterForm()
    {
        var form = renderRegisterFormComponent();
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

    private Section renderRegisterFormComponent() =>
        new Section()
            .Id("register-form")
            .Children(
                new Form()
                    .Action("/interaction/register/create")
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
                                new Field()
                                    .Label(new Label("Confirm Password").For("confirmPassword"))
                                    .Input(new Input()
                                        .Name("confirmPassword")
                                        .Type(InputType.Password)
                                        .Placeholder("Confirm password")
                                        .Id("confirmPassword")
                                    ),
                                new Button(Importance.Primary, "Create Account")
                                    .Type(ButtonType.Submit)
                            )
                    )
            );
}
