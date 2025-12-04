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
    public string ElementName { get; init; }
    public InputType ElementType { get; init; } = InputType.Text;
    public string ElementPlaceholder { get; init; }
    public string ElementValue { get; init; }
    public bool ElementDisabled { get; init; }
    public bool ElementReadOnly { get; init; }
    public string ElementId { get; init; }

    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();

    // Fluent builder methods

    /// <summary>Sets the input name. Returns new instance (immutable).</summary>
    public Input Name(string name) => this with { ElementName = name };

    /// <summary>Sets the input type. Returns new instance (immutable).</summary>
    public Input Type(InputType type) => this with { ElementType = type };

    /// <summary>Sets the placeholder text. Returns new instance (immutable).</summary>
    public Input Placeholder(string placeholder) => this with { ElementPlaceholder = placeholder };

    /// <summary>Sets the input value. Returns new instance (immutable).</summary>
    public Input Value(string value) => this with { ElementValue = value };

    /// <summary>Sets disabled state. Returns new instance (immutable).</summary>
    public Input Disabled(bool disabled) => this with { ElementDisabled = disabled };

    /// <summary>Sets readonly state. Returns new instance (immutable).</summary>
    public Input ReadOnly(bool readOnly) => this with { ElementReadOnly = readOnly };

    /// <summary>Sets the element ID. Returns new instance (immutable).</summary>
    public Input Id(string id) => this with { ElementId = id };
}

/// <summary>
/// Label - Accessible label for form inputs.
/// Associates with input via For property (matches input name).
/// </summary>
public record Label : IFieldContent, IBodyContent
{
    private readonly string _text;

    public string Text => _text;
    public string ElementFor { get; init; }

    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();

    public Label(string text)
    {
        _text = text;
    }

    // Fluent builder methods

    /// <summary>Sets the 'for' attribute to associate with input. Returns new instance (immutable).</summary>
    public Label For(string forAttribute) => this with { ElementFor = forAttribute };
}

/// <summary>
/// Field - Semantic grouping of Label + Input with server-side error display.
/// Displays validation errors returned from workflow via FluentValidation.
/// </summary>
public record Field : ComponentBase, IBodyContent
{
    public Label ElementLabel { get; init; }
    public Input ElementInput { get; init; }
    public Text ElementHelpText { get; init; }
    public string ElementErrorMessage { get; init; }
    public bool ElementHasError { get; init; }

    // Fluent builder methods

    /// <summary>Sets the label. Returns new instance (immutable).</summary>
    public Field Label(Label label) => this with { ElementLabel = label };

    /// <summary>Sets the input. Returns new instance (immutable).</summary>
    public Field Input(Input input) => this with { ElementInput = input };

    /// <summary>Sets the help text. Returns new instance (immutable).</summary>
    public Field HelpText(Text helpText) => this with { ElementHelpText = helpText };

    /// <summary>Sets the error message. Returns new instance (immutable).</summary>
    public Field ErrorMessage(string errorMessage) => this with { ElementErrorMessage = errorMessage };

    /// <summary>Sets the error state. Returns new instance (immutable).</summary>
    public Field HasError(bool hasError) => this with { ElementHasError = hasError };
}

/// <summary>
/// Form - Form container with HTMX support for server interactions.
/// </summary>
public record Form : ComponentBase, IBodyContent
{
    public string ElementAction { get; init; }
    public string ElementMethod { get; init; } = "post";
    public string ElementId { get; init; }
    public string ElementTarget { get; init; }
    public SwapStrategy ElementSwap { get; init; } = SwapStrategy.InnerHTML;

    // Fluent builder methods

    /// <summary>Sets the form action URL. Returns new instance (immutable).</summary>
    public Form Action(string action) => this with { ElementAction = action };

    /// <summary>Sets the HTTP method. Returns new instance (immutable).</summary>
    public Form Method(string method) => this with { ElementMethod = method };

    /// <summary>Sets the element ID. Returns new instance (immutable).</summary>
    public Form Id(string id) => this with { ElementId = id };

    /// <summary>Sets the HTMX target selector. Returns new instance (immutable).</summary>
    public Form Target(string target) => this with { ElementTarget = target };

    /// <summary>Sets the HTMX swap strategy. Returns new instance (immutable).</summary>
    public Form Swap(SwapStrategy swap) => this with { ElementSwap = swap };

    /// <summary>Adds child components. Returns this instance (mutable for children).</summary>
    public new Form Children(params IComponent[] children)
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
    public string ElementName { get; init; }
    public bool ElementChecked { get; init; }
    public string ElementValue { get; init; }
    public bool ElementDisabled { get; init; }
    public string ElementId { get; init; }

    // HTMX support for interactive checkboxes (e.g., todo toggle)
    public string ElementAction { get; init; }
    public string ElementTarget { get; init; }
    public SwapStrategy ElementSwap { get; init; } = SwapStrategy.InnerHTML;
    public long? ElementModelId { get; init; }

    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();

    // Fluent builder methods

    /// <summary>Sets the checkbox name. Returns new instance (immutable).</summary>
    public Checkbox Name(string name) => this with { ElementName = name };

    /// <summary>Sets the checked state. Returns new instance (immutable).</summary>
    public Checkbox Checked(bool checked_) => this with { ElementChecked = checked_ };

    /// <summary>Sets the checkbox value. Returns new instance (immutable).</summary>
    public Checkbox Value(string value) => this with { ElementValue = value };

    /// <summary>Sets disabled state. Returns new instance (immutable).</summary>
    public Checkbox Disabled(bool disabled) => this with { ElementDisabled = disabled };

    /// <summary>Sets the element ID. Returns new instance (immutable).</summary>
    public Checkbox Id(string id) => this with { ElementId = id };

    /// <summary>Sets the HTMX action URL. Returns new instance (immutable).</summary>
    public Checkbox Action(string action) => this with { ElementAction = action };

    /// <summary>Sets the HTMX target selector. Returns new instance (immutable).</summary>
    public Checkbox Target(string target) => this with { ElementTarget = target };

    /// <summary>Sets the HTMX swap strategy. Returns new instance (immutable).</summary>
    public Checkbox Swap(SwapStrategy swap) => this with { ElementSwap = swap };

    /// <summary>Sets the model ID for HTMX interactions. Returns new instance (immutable).</summary>
    public Checkbox ModelId(long? modelId) => this with { ElementModelId = modelId };
}
