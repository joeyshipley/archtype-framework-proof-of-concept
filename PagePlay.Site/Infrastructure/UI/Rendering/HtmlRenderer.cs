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

        sb.Append($"<button class=\"{classes}\"{disabledAttr}>");
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
