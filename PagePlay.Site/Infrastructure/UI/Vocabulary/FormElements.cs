namespace PagePlay.Site.Infrastructure.UI.Vocabulary;

/// <summary>
/// Input type - Semantic intent for different input types.
/// Maps to HTML input type attribute and provides mobile keyboard optimization.
/// </summary>
public enum InputType
{
    Text,
    Email,
    Password,
    Hidden,
    Number,
    Date,
    Tel,
    Url,
    Search
}

/// <summary>
/// Input - Core form input element.
/// Declares semantic type (email, password, etc.) without client-side validation duplication.
/// Server validates via FluentValidation in workflow validators.
/// </summary>
public record Input : IFieldContent, IBodyContent
{
    public required string Name { get; init; }
    public InputType Type { get; init; } = InputType.Text;
    public string Placeholder { get; init; }
    public string Value { get; init; }
    public bool Disabled { get; init; }
    public bool ReadOnly { get; init; }
    public string Id { get; init; }

    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();
}

/// <summary>
/// Label - Accessible label for form inputs.
/// Associates with input via For property (matches input name).
/// </summary>
public record Label : IFieldContent, IBodyContent
{
    private readonly string _text;

    public string Text => _text;
    public string For { get; init; }

    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();

    public Label(string text)
    {
        _text = text;
    }
}

/// <summary>
/// Field - Semantic grouping of Label + Input with server-side error display.
/// Displays validation errors returned from workflow via FluentValidation.
/// </summary>
public record Field : ComponentBase, IBodyContent
{
    public Label Label { get; init; }
    public required Input Input { get; init; }
    public Text HelpText { get; init; }
    public string ErrorMessage { get; init; }
    public bool HasError { get; init; }
}

/// <summary>
/// Form - Form container with HTMX support for server interactions.
/// </summary>
public record Form : ComponentBase, IBodyContent
{
    public required string Action { get; init; }
    public string Method { get; init; } = "post";
    public string Id { get; init; }
    public string Target { get; init; }
    public SwapStrategy Swap { get; init; } = SwapStrategy.InnerHTML;
}

/// <summary>
/// Checkbox - Checkbox input with HTMX support for interactive toggles.
/// Can be used in traditional forms or with HTMX for immediate server interaction.
/// </summary>
public record Checkbox : IFieldContent, IBodyContent
{
    public required string Name { get; init; }
    public bool Checked { get; init; }
    public string Value { get; init; }
    public bool Disabled { get; init; }
    public string Id { get; init; }

    // HTMX support for interactive checkboxes (e.g., todo toggle)
    public string Action { get; init; }
    public string Target { get; init; }
    public SwapStrategy Swap { get; init; } = SwapStrategy.InnerHTML;
    public long? ModelId { get; init; }

    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();
}
