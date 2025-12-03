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
    public string Name { get; init; }
    public InputType Type { get; init; } = InputType.Text;
    public string Placeholder { get; init; }
    public string Value { get; init; }
    public bool Disabled { get; init; }
    public bool ReadOnly { get; init; }
    public string Id { get; init; }

    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();

    // Fluent builder methods

    /// <summary>Sets the input name. Returns new instance (immutable).</summary>
    public Input WithName(string name) => this with { Name = name };

    /// <summary>Sets the input type. Returns new instance (immutable).</summary>
    public Input WithType(InputType type) => this with { Type = type };

    /// <summary>Sets the placeholder text. Returns new instance (immutable).</summary>
    public Input WithPlaceholder(string placeholder) => this with { Placeholder = placeholder };

    /// <summary>Sets the input value. Returns new instance (immutable).</summary>
    public Input WithValue(string value) => this with { Value = value };

    /// <summary>Sets disabled state. Returns new instance (immutable).</summary>
    public Input WithDisabled(bool disabled) => this with { Disabled = disabled };

    /// <summary>Sets readonly state. Returns new instance (immutable).</summary>
    public Input WithReadOnly(bool readOnly) => this with { ReadOnly = readOnly };

    /// <summary>Sets the element ID. Returns new instance (immutable).</summary>
    public Input WithId(string id) => this with { Id = id };
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

    // Fluent builder methods

    /// <summary>Sets the 'for' attribute to associate with input. Returns new instance (immutable).</summary>
    public Label WithFor(string forAttribute) => this with { For = forAttribute };
}

/// <summary>
/// Field - Semantic grouping of Label + Input with server-side error display.
/// Displays validation errors returned from workflow via FluentValidation.
/// </summary>
public record Field : ComponentBase, IBodyContent
{
    public Label Label { get; init; }
    public Input Input { get; init; }
    public Text HelpText { get; init; }
    public string ErrorMessage { get; init; }
    public bool HasError { get; init; }

    // Fluent builder methods

    /// <summary>Sets the label. Returns new instance (immutable).</summary>
    public Field WithLabel(Label label) => this with { Label = label };

    /// <summary>Sets the input. Returns new instance (immutable).</summary>
    public Field WithInput(Input input) => this with { Input = input };

    /// <summary>Sets the help text. Returns new instance (immutable).</summary>
    public Field WithHelpText(Text helpText) => this with { HelpText = helpText };

    /// <summary>Sets the error message. Returns new instance (immutable).</summary>
    public Field WithErrorMessage(string errorMessage) => this with { ErrorMessage = errorMessage };

    /// <summary>Sets the error state. Returns new instance (immutable).</summary>
    public Field WithHasError(bool hasError) => this with { HasError = hasError };
}

/// <summary>
/// Form - Form container with HTMX support for server interactions.
/// </summary>
public record Form : ComponentBase, IBodyContent
{
    public string Action { get; init; }
    public string Method { get; init; } = "post";
    public string Id { get; init; }
    public string Target { get; init; }
    public SwapStrategy Swap { get; init; } = SwapStrategy.InnerHTML;

    // Fluent builder methods

    /// <summary>Sets the form action URL. Returns new instance (immutable).</summary>
    public Form WithAction(string action) => this with { Action = action };

    /// <summary>Sets the HTTP method. Returns new instance (immutable).</summary>
    public Form WithMethod(string method) => this with { Method = method };

    /// <summary>Sets the element ID. Returns new instance (immutable).</summary>
    public Form WithId(string id) => this with { Id = id };

    /// <summary>Sets the HTMX target selector. Returns new instance (immutable).</summary>
    public Form WithTarget(string target) => this with { Target = target };

    /// <summary>Sets the HTMX swap strategy. Returns new instance (immutable).</summary>
    public Form WithSwap(SwapStrategy swap) => this with { Swap = swap };

    /// <summary>Adds child components. Returns this instance (mutable for children).</summary>
    public Form WithChildren(params IComponent[] children)
    {
        foreach (var child in children)
            Add(child);
        return this;
    }
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
