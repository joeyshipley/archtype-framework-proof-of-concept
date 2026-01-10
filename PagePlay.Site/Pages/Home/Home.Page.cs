using PagePlay.Site.Infrastructure.UI;
using PagePlay.Site.Infrastructure.UI.Rendering;
using PagePlay.Site.Infrastructure.UI.Vocabulary;
using PagePlay.Site.Infrastructure.Web.Components;

namespace PagePlay.Site.Pages.Home;

public interface IHomePageView : IView
{
}

public class HomePage(IHtmlRenderer _renderer) : IHomePageView
{
    public string ViewId => "home-page";

    public DataDependencies Dependencies => DataDependencies.None;

    public string Render(IDataContext data)
    {
        var page = new Page(
            new PageTitle("PagePlay"),

            // Button Showcase Section
            new Section(
                new SectionTitle("Button Variants"),
                new Card()
                    .Body(
                        new Stack(For.Fields,
                            new Text("Primary, Secondary, Tertiary, and Ghost button styles:"),
                            new Row(For.Actions,
                                new Button(Importance.Primary, "Primary"),
                                new Button(Importance.Secondary, "Secondary"),
                                new Button(Importance.Tertiary, "Tertiary"),
                                new Button(Importance.Ghost, "Ghost")
                            ),
                            new Text("Disabled states:"),
                            new Row(For.Actions,
                                new Button(Importance.Primary, "Primary").Disabled(true),
                                new Button(Importance.Secondary, "Secondary").Disabled(true)
                            )
                        )
                    )
            ),

            // Alert Showcase Section
            new Section(
                new SectionTitle("Alert Variants"),
                new Stack(For.Fields,
                    new Alert("This is a neutral alert for general information.", AlertTone.Neutral),
                    new Alert("Success! Your changes have been saved.", AlertTone.Positive),
                    new Alert("Warning: This action cannot be undone.", AlertTone.Warning),
                    new Alert("Error: Please check your input and try again.", AlertTone.Critical)
                )
            ),

            // Form Elements Section
            new Section(
                new SectionTitle("Form Elements"),
                new Card()
                    .Header(new Text("Sample Form"))
                    .Body(
                        new Stack(For.Fields,
                            new Field()
                                .Label(new Label("Email Address").For("email"))
                                .Input(new Input().Name("email").Type(InputType.Email).Placeholder("you@example.com").Id("email"))
                                .HelpText(new Text("We'll never share your email.")),
                            new Field()
                                .Label(new Label("Password").For("password"))
                                .Input(new Input().Name("password").Type(InputType.Password).Placeholder("Enter your password").Id("password")),
                            new Field()
                                .Label(new Label("Username (with error)").For("username"))
                                .Input(new Input().Name("username").Value("taken_user").Id("username"))
                                .ErrorMessage("This username is already taken.")
                                .HasError(true),
                            new Field()
                                .Label(new Label("Disabled Input").For("disabled"))
                                .Input(new Input().Name("disabled").Value("Cannot edit this").Disabled(true).Id("disabled")),
                            new Row(For.Inline,
                                new Checkbox().Name("remember").Id("remember"),
                                new Label("Remember me").For("remember")
                            )
                        )
                    )
                    .Footer(
                        new Button(Importance.Secondary, "Cancel"),
                        new Button(Importance.Primary, "Submit").Type(ButtonType.Submit)
                    )
            ),

            // Card Showcase Section
            new Section(
                new SectionTitle("Card Layouts"),
                new Grid(For.Cards, Columns.Two,
                    new Card()
                        .Header(
                            new Text("Card with Header & Footer")
                        )
                        .Body(
                            new Text("This card demonstrates the Flowbite-style border approach."),
                            new Text("Notice the subtle borders between header, body, and footer.")
                        )
                        .Footer(
                            new Button(Importance.Secondary, "Cancel"),
                            new Button(Importance.Primary, "Save")
                        ),
                    new Card()
                        .Header(
                            new Text("Card with Header Only")
                        )
                        .Body(
                            new Text("This card has a header but no footer."),
                            new Text("The header border separates it from the body content.")
                        ),
                    new Card()
                        .Body(
                            new Text("Card without Header or Footer"),
                            new Text("A simple content-only card with just a border around it.")
                        ),
                    new Card()
                        .Body(
                            new Text("Another simple card to show the grid layout.")
                        )
                        .Footer(
                            new Button(Importance.Primary, "Action")
                        )
                )
            ),

            // Layout Spacing Showcase - Visual blocks to see gaps
            new Section(
                new SectionTitle("Layout Spacing"),
                new Stack(For.Fields,
                    // Row gaps comparison - side by side
                    new Card()
                        .Header(new Text("Row Gaps"))
                        .Body(
                            new Stack(For.Content,
                                new Text("Inline"),
                                new Row(For.Inline,
                                    new Card().Body(new Text("A")),
                                    new Card().Body(new Text("B")),
                                    new Card().Body(new Text("C"))
                                ),
                                new Text("Actions"),
                                new Row(For.Actions,
                                    new Card().Body(new Text("A")),
                                    new Card().Body(new Text("B")),
                                    new Card().Body(new Text("C"))
                                ),
                                new Text("Items"),
                                new Row(For.Items,
                                    new Card().Body(new Text("A")),
                                    new Card().Body(new Text("B")),
                                    new Card().Body(new Text("C"))
                                ),
                                new Text("Fields"),
                                new Row(For.Fields,
                                    new Card().Body(new Text("A")),
                                    new Card().Body(new Text("B")),
                                    new Card().Body(new Text("C"))
                                ),
                                new Text("Cards"),
                                new Row(For.Cards,
                                    new Card().Body(new Text("A")),
                                    new Card().Body(new Text("B")),
                                    new Card().Body(new Text("C"))
                                )
                            )
                        ),

                    // Grid columns
                    new Card()
                        .Header(new Text("Grid Columns"))
                        .Body(
                            new Stack(For.Content,
                                new Text("Two"),
                                new Grid(For.Items, Columns.Two,
                                    new Card().Body(new Text("1")),
                                    new Card().Body(new Text("2")),
                                    new Card().Body(new Text("3")),
                                    new Card().Body(new Text("4"))
                                ),
                                new Text("Three"),
                                new Grid(For.Items, Columns.Three,
                                    new Card().Body(new Text("1")),
                                    new Card().Body(new Text("2")),
                                    new Card().Body(new Text("3"))
                                ),
                                new Text("Four"),
                                new Grid(For.Items, Columns.Four,
                                    new Card().Body(new Text("1")),
                                    new Card().Body(new Text("2")),
                                    new Card().Body(new Text("3")),
                                    new Card().Body(new Text("4"))
                                ),
                                new Text("Auto (responsive)"),
                                new Grid(For.Cards, Columns.Auto,
                                    new Card().Body(new Text("Auto 1")),
                                    new Card().Body(new Text("Auto 2")),
                                    new Card().Body(new Text("Auto 3"))
                                )
                            )
                        )
                )
            ),

            // List Showcase Section
            new Section(
                new SectionTitle("List Elements"),
                new Grid(For.Cards, Columns.Two,
                    // Plain list (dashboard style)
                    new Card()
                        .Header(new Text("Plain List (Dashboard Style)"))
                        .Body(
                            new List(
                                new ListItem(new Text("First dashboard item")),
                                new ListItem(new Text("Second dashboard item")),
                                new ListItem(new Text("Third dashboard item")),
                                new ListItem(new Text("Fourth dashboard item"))
                            ).Style(ListStyle.Plain)
                        ),

                    // List item states
                    new Card()
                        .Header(new Text("List Item States"))
                        .Body(
                            new List(
                                new ListItem(new Text("Normal item")).State(ListItemState.Normal),
                                new ListItem(new Text("Completed item - strikethrough")).State(ListItemState.Completed),
                                new ListItem(new Text("Disabled item - muted")).State(ListItemState.Disabled),
                                new ListItem(new Text("Error item - highlighted")).State(ListItemState.Error)
                            ).Style(ListStyle.Plain)
                        ),

                    // Unordered list
                    new Card()
                        .Header(new Text("Unordered List"))
                        .Body(
                            new List(
                                new ListItem(new Text("First bullet point")),
                                new ListItem(new Text("Second bullet point")),
                                new ListItem(new Text("Third bullet point"))
                            ).Style(ListStyle.Unordered)
                        ),

                    // Ordered list
                    new Card()
                        .Header(new Text("Ordered List"))
                        .Body(
                            new List(
                                new ListItem(new Text("Step one")),
                                new ListItem(new Text("Step two")),
                                new ListItem(new Text("Step three"))
                            ).Style(ListStyle.Ordered)
                        )
                )
            )
        ).Id(ViewId);

        return _renderer.Render(page);
    }
}
