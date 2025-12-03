using System.Text;
using PagePlay.Site.Infrastructure.UI.Vocabulary;

namespace PagePlay.Site.Infrastructure.UI.Rendering;

/// <summary>
/// Renders semantic UI components to clean HTML with semantic class names.
/// No inline styles. No className escape hatches. Theme controls all appearance via CSS.
/// </summary>
public interface IHtmlRenderer
{
    string Render(IComponent component);
}

public class HtmlRenderer : IHtmlRenderer
{
    public string Render(IComponent component)
    {
        var builder = new StringBuilder();
        renderComponent(component, builder);
        return builder.ToString();
    }

    private void renderComponent(IComponent component, StringBuilder sb)
    {
        switch (component)
        {
            case Page page:
                renderPage(page, sb);
                break;
            case Section section:
                renderSection(section, sb);
                break;
            case PageTitle pageTitle:
                renderPageTitle(pageTitle, sb);
                break;
            case SectionTitle sectionTitle:
                renderSectionTitle(sectionTitle, sb);
                break;
            case Stack stack:
                renderStack(stack, sb);
                break;
            case Row row:
                renderRow(row, sb);
                break;
            case Grid grid:
                renderGrid(grid, sb);
                break;
            case Card card:
                renderCard(card, sb);
                break;
            case Header header:
                renderSlot("header", header, sb);
                break;
            case Body body:
                renderSlot("body", body, sb);
                break;
            case Footer footer:
                renderSlot("footer", footer, sb);
                break;
            case Text text:
                renderText(text, sb);
                break;
            case Button button:
                renderButton(button, sb);
                break;
            case Input input:
                renderInput(input, sb);
                break;
            case Label label:
                renderLabel(label, sb);
                break;
            case Field field:
                renderField(field, sb);
                break;
            case Form form:
                renderForm(form, sb);
                break;
            case Checkbox checkbox:
                renderCheckbox(checkbox, sb);
                break;
            default:
                throw new InvalidOperationException($"Unknown component type: {component.GetType().Name}");
        }
    }

    private void renderCard(Card card, StringBuilder sb)
    {
        sb.Append("<div class=\"card\">");

        if (card.Header != null)
            renderComponent(card.Header, sb);

        renderComponent(card.Body, sb);

        if (card.Footer != null)
            renderComponent(card.Footer, sb);

        sb.Append("</div>");
    }

    private void renderSlot(string slotName, ComponentBase slot, StringBuilder sb)
    {
        sb.Append($"<div class=\"{slotName}\">");

        foreach (var child in slot.Children)
            renderComponent(child, sb);

        sb.Append("</div>");
    }

    private void renderText(Text text, StringBuilder sb)
    {
        sb.Append("<p class=\"text\">");
        sb.Append(htmlEncode(text.Content));
        sb.Append("</p>");
    }

    private void renderButton(Button button, StringBuilder sb)
    {
        var classes = "button";

        // Add importance modifier
        var importanceModifier = button.Importance switch
        {
            Importance.Primary => "button--primary",
            Importance.Secondary => "button--secondary",
            Importance.Tertiary => "button--tertiary",
            Importance.Ghost => "button--ghost",
            _ => "button--secondary"
        };

        classes = $"{classes} {importanceModifier}";

        if (button.Disabled)
            classes += " button--disabled";

        if (button.Loading)
            classes += " button--loading";

        var disabledAttr = button.Disabled ? " disabled" : "";

        // Build HTMX attributes if Action is specified
        var htmxAttrs = "";
        if (!string.IsNullOrEmpty(button.Action))
        {
            htmxAttrs = $" hx-post=\"{htmlEncode(button.Action)}\"";

            if (!string.IsNullOrEmpty(button.Target))
                htmxAttrs += $" hx-target=\"{htmlEncode(button.Target)}\"";

            var swapValue = button.Swap switch
            {
                SwapStrategy.InnerHTML => "innerHTML",
                SwapStrategy.OuterHTML => "outerHTML",
                SwapStrategy.BeforeBegin => "beforebegin",
                SwapStrategy.AfterBegin => "afterbegin",
                SwapStrategy.BeforeEnd => "beforeend",
                SwapStrategy.AfterEnd => "afterend",
                _ => "innerHTML"
            };
            htmxAttrs += $" hx-swap=\"{swapValue}\"";

            // Always include hx-vals to ensure POST has a body (required for [FromForm] binding)
            // Include ModelId if specified, otherwise send dummy field to create non-empty body
            var hxValsContent = button.ModelId.HasValue
                ? $"{{\"id\": {button.ModelId.Value}}}"
                : "{\"_\":\"\"}";
            htmxAttrs += $" hx-vals='{hxValsContent}'";
        }

        var idAttr = !string.IsNullOrEmpty(button.Id) ? $" id=\"{htmlEncode(button.Id)}\"" : "";

        var typeAttr = button.Type switch
        {
            ButtonType.Submit => " type=\"submit\"",
            ButtonType.Reset => " type=\"reset\"",
            ButtonType.Button => " type=\"button\"",
            _ => " type=\"button\""
        };

        sb.Append($"<button class=\"{classes}\"{idAttr}{typeAttr}{htmxAttrs}{disabledAttr}>");
        sb.Append(htmlEncode(button.Label));
        sb.Append("</button>");
    }

    private void renderPage(Page page, StringBuilder sb)
    {
        sb.Append("<div class=\"page\">");

        foreach (var child in page.Children)
            renderComponent(child, sb);

        sb.Append("</div>");
    }

    private void renderSection(Section section, StringBuilder sb)
    {
        sb.Append("<section class=\"section\">");

        foreach (var child in section.Children)
            renderComponent(child, sb);

        sb.Append("</section>");
    }

    private void renderPageTitle(PageTitle pageTitle, StringBuilder sb)
    {
        sb.Append("<h1 class=\"page-title\">");
        sb.Append(htmlEncode(pageTitle.Title));
        sb.Append("</h1>");
    }

    private void renderSectionTitle(SectionTitle sectionTitle, StringBuilder sb)
    {
        sb.Append("<h2 class=\"section-title\">");
        sb.Append(htmlEncode(sectionTitle.Title));
        sb.Append("</h2>");
    }

    private void renderStack(Stack stack, StringBuilder sb)
    {
        var purposeClass = getPurposeClass(stack.Purpose);
        sb.Append($"<div class=\"stack stack--{purposeClass}\">");

        foreach (var child in stack.Children)
            renderComponent(child, sb);

        sb.Append("</div>");
    }

    private void renderRow(Row row, StringBuilder sb)
    {
        var purposeClass = getPurposeClass(row.Purpose);
        sb.Append($"<div class=\"row row--{purposeClass}\">");

        foreach (var child in row.Children)
            renderComponent(child, sb);

        sb.Append("</div>");
    }

    private void renderGrid(Grid grid, StringBuilder sb)
    {
        var purposeClass = getPurposeClass(grid.Purpose);
        var columnsClass = getColumnsClass(grid.Columns);
        sb.Append($"<div class=\"grid grid--{purposeClass} grid--{columnsClass}\">");

        foreach (var child in grid.Children)
            renderComponent(child, sb);

        sb.Append("</div>");
    }

    private string getPurposeClass(For purpose) => purpose switch
    {
        For.Actions => "actions",
        For.Fields => "fields",
        For.Content => "content",
        For.Items => "items",
        For.Sections => "sections",
        For.Inline => "inline",
        For.Cards => "cards",
        _ => "items"
    };

    private string getColumnsClass(Columns columns) => columns switch
    {
        Columns.One => "cols-1",
        Columns.Two => "cols-2",
        Columns.Three => "cols-3",
        Columns.Four => "cols-4",
        Columns.Auto => "cols-auto",
        _ => "cols-auto"
    };

    private void renderInput(Input input, StringBuilder sb)
    {
        var typeValue = input.Type switch
        {
            InputType.Text => "text",
            InputType.Email => "email",
            InputType.Password => "password",
            InputType.Hidden => "hidden",
            InputType.Number => "number",
            InputType.Date => "date",
            InputType.Tel => "tel",
            InputType.Url => "url",
            InputType.Search => "search",
            _ => "text"
        };

        var classes = $"input input--{typeValue}";
        var nameAttr = $" name=\"{htmlEncode(input.Name)}\"";
        var typeAttr = $" type=\"{typeValue}\"";
        var placeholderAttr = !string.IsNullOrEmpty(input.Placeholder) ? $" placeholder=\"{htmlEncode(input.Placeholder)}\"" : "";
        var valueAttr = !string.IsNullOrEmpty(input.Value) ? $" value=\"{htmlEncode(input.Value)}\"" : "";
        var disabledAttr = input.Disabled ? " disabled" : "";
        var readonlyAttr = input.ReadOnly ? " readonly" : "";
        var idAttr = !string.IsNullOrEmpty(input.Id) ? $" id=\"{htmlEncode(input.Id)}\"" : "";

        sb.Append($"<input class=\"{classes}\"{idAttr}{nameAttr}{typeAttr}{placeholderAttr}{valueAttr}{disabledAttr}{readonlyAttr} />");
    }

    private void renderLabel(Label label, StringBuilder sb)
    {
        var forAttr = !string.IsNullOrEmpty(label.For) ? $" for=\"{htmlEncode(label.For)}\"" : "";

        sb.Append($"<label class=\"label\"{forAttr}>");
        sb.Append(htmlEncode(label.Text));
        sb.Append("</label>");
    }

    private void renderField(Field field, StringBuilder sb)
    {
        var classes = field.HasError ? "field field--error" : "field";

        sb.Append($"<div class=\"{classes}\">");

        if (field.Label != null)
            renderComponent(field.Label, sb);

        renderComponent(field.Input, sb);

        if (field.HelpText != null && !field.HasError)
        {
            sb.Append("<p class=\"field__help\">");
            sb.Append(htmlEncode(field.HelpText.Content));
            sb.Append("</p>");
        }

        if (field.HasError && !string.IsNullOrEmpty(field.ErrorMessage))
        {
            sb.Append("<p class=\"field__error\">");
            sb.Append(htmlEncode(field.ErrorMessage));
            sb.Append("</p>");
        }

        sb.Append("</div>");
    }

    private void renderForm(Form form, StringBuilder sb)
    {
        var idAttr = !string.IsNullOrEmpty(form.Id) ? $" id=\"{htmlEncode(form.Id)}\"" : "";
        var methodAttr = $" method=\"{form.Method}\"";

        // Build HTMX attributes
        var htmxMethod = form.Method.ToLower() == "post" ? "hx-post" : "hx-get";
        var htmxAttrs = $" {htmxMethod}=\"{htmlEncode(form.Action)}\"";

        if (!string.IsNullOrEmpty(form.Target))
            htmxAttrs += $" hx-target=\"{htmlEncode(form.Target)}\"";

        var swapValue = form.Swap switch
        {
            SwapStrategy.InnerHTML => "innerHTML",
            SwapStrategy.OuterHTML => "outerHTML",
            SwapStrategy.BeforeBegin => "beforebegin",
            SwapStrategy.AfterBegin => "afterbegin",
            SwapStrategy.BeforeEnd => "beforeend",
            SwapStrategy.AfterEnd => "afterend",
            _ => "innerHTML"
        };
        htmxAttrs += $" hx-swap=\"{swapValue}\"";

        sb.Append($"<form class=\"form\"{idAttr}{htmxAttrs}>");

        foreach (var child in form.Children)
            renderComponent(child, sb);

        sb.Append("</form>");
    }

    private void renderCheckbox(Checkbox checkbox, StringBuilder sb)
    {
        var nameAttr = $" name=\"{htmlEncode(checkbox.Name)}\"";
        var checkedAttr = checkbox.Checked ? " checked" : "";
        var valueAttr = !string.IsNullOrEmpty(checkbox.Value) ? $" value=\"{htmlEncode(checkbox.Value)}\"" : "";
        var disabledAttr = checkbox.Disabled ? " disabled" : "";
        var idAttr = !string.IsNullOrEmpty(checkbox.Id) ? $" id=\"{htmlEncode(checkbox.Id)}\"" : "";

        // Build HTMX attributes if Action is specified
        var htmxAttrs = "";
        if (!string.IsNullOrEmpty(checkbox.Action))
        {
            htmxAttrs = $" hx-post=\"{htmlEncode(checkbox.Action)}\"";

            if (!string.IsNullOrEmpty(checkbox.Target))
                htmxAttrs += $" hx-target=\"{htmlEncode(checkbox.Target)}\"";

            var swapValue = checkbox.Swap switch
            {
                SwapStrategy.InnerHTML => "innerHTML",
                SwapStrategy.OuterHTML => "outerHTML",
                SwapStrategy.BeforeBegin => "beforebegin",
                SwapStrategy.AfterBegin => "afterbegin",
                SwapStrategy.BeforeEnd => "beforeend",
                SwapStrategy.AfterEnd => "afterend",
                _ => "innerHTML"
            };
            htmxAttrs += $" hx-swap=\"{swapValue}\"";

            // Always include hx-vals for ModelId
            var hxValsContent = checkbox.ModelId.HasValue
                ? $"{{\"id\": {checkbox.ModelId.Value}}}"
                : "{\"_\":\"\"}";
            htmxAttrs += $" hx-vals='{hxValsContent}'";

            // Trigger on change for checkboxes
            htmxAttrs += " hx-trigger=\"change\"";
        }

        sb.Append($"<input type=\"checkbox\" class=\"checkbox\"{idAttr}{nameAttr}{checkedAttr}{valueAttr}{htmxAttrs}{disabledAttr} />");
    }

    private string htmlEncode(string text)
    {
        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
    }
}
