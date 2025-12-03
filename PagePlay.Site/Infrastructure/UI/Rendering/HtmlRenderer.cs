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
