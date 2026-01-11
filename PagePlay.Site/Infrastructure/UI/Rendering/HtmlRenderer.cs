using System.Text;
using PagePlay.Site.Infrastructure.UI.Vocabulary;

namespace PagePlay.Site.Infrastructure.UI.Rendering;

/// <summary>
/// Renders semantic UI components to clean HTML with semantic class names.
/// No inline styles. No className escape hatches. Theme controls all appearance via CSS.
/// </summary>
public interface IHtmlRenderer
{
    string Render(IElement element);
}

public class HtmlRenderer : IHtmlRenderer
{
    public string Render(IElement element)
    {
        var builder = new StringBuilder();
        renderElement(element, builder);
        return builder.ToString();
    }

    private void renderElement(IElement element, StringBuilder sb)
    {
        switch (element)
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
            case Alert alert:
                renderAlert(alert, sb);
                break;
            case EmptyState emptyState:
                renderEmptyState(emptyState, sb);
                break;
            case List list:
                renderList(list, sb);
                break;
            case ListItem listItem:
                renderListItem(listItem, sb);
                break;
            case Tabs tabs:
                renderTabs(tabs, sb);
                break;
            case Badge badge:
                renderBadge(badge, sb);
                break;
            case TopNav topNav:
                renderTopNav(topNav, sb);
                break;
            case Link link:
                renderLink(link, sb);
                break;
            default:
                throw new InvalidOperationException($"Unknown element type: {element.GetType().Name}");
        }
    }

    private void renderCard(Card card, StringBuilder sb)
    {
        sb.Append("<div class=\"card\">");

        if (card._headerSlot != null)
            renderElement(card._headerSlot, sb);

        if (card._bodySlot != null)
            renderElement(card._bodySlot, sb);

        if (card._footerSlot != null)
            renderElement(card._footerSlot, sb);

        sb.Append("</div>");
    }

    private void renderSlot(string slotName, ElementBase slot, StringBuilder sb)
    {
        sb.Append($"<div class=\"{slotName}\">");

        foreach (var child in slot.Children)
            renderElement(child, sb);

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

        if (button.ElementDisabled)
            classes += " button--disabled";

        if (button.ElementLoading)
            classes += " button--loading";

        var disabledAttr = button.ElementDisabled ? " disabled" : "";

        // Build HTMX attributes if Action is specified
        var htmxAttrs = "";
        if (!string.IsNullOrEmpty(button.ElementAction))
        {
            htmxAttrs = $" hx-post=\"{htmlEncode(button.ElementAction)}\"";

            if (!string.IsNullOrEmpty(button.ElementTarget))
                htmxAttrs += $" hx-target=\"{htmlEncode(button.ElementTarget)}\"";

            var swapValue = button.ElementSwap switch
            {
                SwapStrategy.InnerHTML => "innerHTML",
                SwapStrategy.OuterHTML => "outerHTML",
                SwapStrategy.BeforeBegin => "beforebegin",
                SwapStrategy.AfterBegin => "afterbegin",
                SwapStrategy.BeforeEnd => "beforeend",
                SwapStrategy.AfterEnd => "afterend",
                SwapStrategy.None => "none",
                _ => "innerHTML"
            };
            htmxAttrs += $" hx-swap=\"{swapValue}\"";

            // Always include hx-vals to ensure POST has a body (required for [FromForm] binding)
            // Include ModelId if specified, otherwise send dummy field to create non-empty body
            var hxValsContent = button.ElementModelId.HasValue
                ? $"{{\"id\": {button.ElementModelId.Value}}}"
                : "{\"_\":\"\"}";
            htmxAttrs += $" hx-vals='{hxValsContent}'";
        }

        var idAttr = !string.IsNullOrEmpty(button.ElementId) ? $" id=\"{htmlEncode(button.ElementId)}\"" : "";

        var typeAttr = button.ElementType switch
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
        var idAttr = !string.IsNullOrEmpty(page.ElementId) ? $" id=\"{htmlEncode(page.ElementId)}\"" : "";
        sb.Append($"<div class=\"page\"{idAttr}>");

        foreach (var child in ((IElement)page).Children)
            renderElement(child, sb);

        sb.Append("</div>");
    }

    private void renderSection(Section section, StringBuilder sb)
    {
        var idAttr = !string.IsNullOrEmpty(section.ElementId) ? $" id=\"{htmlEncode(section.ElementId)}\"" : "";
        var dropZoneAttr = !string.IsNullOrEmpty(section.DropZoneName) ? $" data-drop-zone=\"{htmlEncode(section.DropZoneName)}\"" : "";
        sb.Append($"<section class=\"section\"{idAttr}{dropZoneAttr}>");

        foreach (var child in ((IElement)section).Children)
            renderElement(child, sb);

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
        var purposeClass = getPurposeClass(stack.ElementPurpose);
        sb.Append($"<div class=\"stack stack--{purposeClass}\">");

        foreach (var child in ((IElement)stack).Children)
            renderElement(child, sb);

        sb.Append("</div>");
    }

    private void renderRow(Row row, StringBuilder sb)
    {
        var purposeClass = getPurposeClass(row.ElementPurpose);
        sb.Append($"<div class=\"row row--{purposeClass}\">");

        foreach (var child in ((IElement)row).Children)
            renderElement(child, sb);

        sb.Append("</div>");
    }

    private void renderGrid(Grid grid, StringBuilder sb)
    {
        var purposeClass = getPurposeClass(grid.ElementPurpose);
        var columnsClass = getColumnsClass(grid.ElementColumns);
        sb.Append($"<div class=\"grid grid--{purposeClass} grid--{columnsClass}\">");

        foreach (var child in ((IElement)grid).Children)
            renderElement(child, sb);

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
        var typeValue = input.ElementType switch
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
        var nameAttr = $" name=\"{htmlEncode(input.ElementName)}\"";
        var typeAttr = $" type=\"{typeValue}\"";
        var placeholderAttr = !string.IsNullOrEmpty(input.ElementPlaceholder) ? $" placeholder=\"{htmlEncode(input.ElementPlaceholder)}\"" : "";
        var valueAttr = !string.IsNullOrEmpty(input.ElementValue) ? $" value=\"{htmlEncode(input.ElementValue)}\"" : "";
        var disabledAttr = input.ElementDisabled ? " disabled" : "";
        var readonlyAttr = input.ElementReadOnly ? " readonly" : "";
        var idAttr = !string.IsNullOrEmpty(input.ElementId) ? $" id=\"{htmlEncode(input.ElementId)}\"" : "";

        sb.Append($"<input class=\"{classes}\"{idAttr}{nameAttr}{typeAttr}{placeholderAttr}{valueAttr}{disabledAttr}{readonlyAttr} />");
    }

    private void renderLabel(Label label, StringBuilder sb)
    {
        var forAttr = !string.IsNullOrEmpty(label.ElementFor) ? $" for=\"{htmlEncode(label.ElementFor)}\"" : "";

        sb.Append($"<label class=\"label\"{forAttr}>");
        sb.Append(htmlEncode(label.Text));
        sb.Append("</label>");
    }

    private void renderField(Field field, StringBuilder sb)
    {
        var classes = field.ElementHasError ? "field field--error" : "field";

        sb.Append($"<div class=\"{classes}\">");

        if (field.ElementLabel != null)
            renderElement(field.ElementLabel, sb);

        renderElement(field.ElementInput, sb);

        if (field.ElementHelpText != null && !field.ElementHasError)
        {
            sb.Append("<p class=\"field__help\">");
            sb.Append(htmlEncode(field.ElementHelpText.Content));
            sb.Append("</p>");
        }

        if (field.ElementHasError && !string.IsNullOrEmpty(field.ElementErrorMessage))
        {
            sb.Append("<p class=\"field__error\">");
            sb.Append(htmlEncode(field.ElementErrorMessage));
            sb.Append("</p>");
        }

        sb.Append("</div>");
    }

    private void renderForm(Form form, StringBuilder sb)
    {
        var idAttr = !string.IsNullOrEmpty(form.ElementId) ? $" id=\"{htmlEncode(form.ElementId)}\"" : "";
        var methodAttr = $" method=\"{form.ElementMethod}\"";

        // Build HTMX attributes
        var htmxMethod = form.ElementMethod.ToLower() == "post" ? "hx-post" : "hx-get";
        var htmxAttrs = $" {htmxMethod}=\"{htmlEncode(form.ElementAction)}\"";

        if (!string.IsNullOrEmpty(form.ElementTarget))
            htmxAttrs += $" hx-target=\"{htmlEncode(form.ElementTarget)}\"";

        var swapValue = form.ElementSwap switch
        {
            SwapStrategy.InnerHTML => "innerHTML",
            SwapStrategy.OuterHTML => "outerHTML",
            SwapStrategy.BeforeBegin => "beforebegin",
            SwapStrategy.AfterBegin => "afterbegin",
            SwapStrategy.BeforeEnd => "beforeend",
            SwapStrategy.AfterEnd => "afterend",
            SwapStrategy.None => "none",
            _ => "innerHTML"
        };
        htmxAttrs += $" hx-swap=\"{swapValue}\"";

        sb.Append($"<form class=\"form\"{idAttr}{htmxAttrs}>");

        foreach (var child in ((IElement)form).Children)
            renderElement(child, sb);

        sb.Append("</form>");
    }

    private void renderCheckbox(Checkbox checkbox, StringBuilder sb)
    {
        var nameAttr = $" name=\"{htmlEncode(checkbox.ElementName)}\"";
        var checkedAttr = checkbox.ElementChecked ? " checked" : "";
        var valueAttr = !string.IsNullOrEmpty(checkbox.ElementValue) ? $" value=\"{htmlEncode(checkbox.ElementValue)}\"" : "";
        var disabledAttr = checkbox.ElementDisabled ? " disabled" : "";
        var idAttr = !string.IsNullOrEmpty(checkbox.ElementId) ? $" id=\"{htmlEncode(checkbox.ElementId)}\"" : "";

        // Build HTMX attributes if Action is specified
        var htmxAttrs = "";
        if (!string.IsNullOrEmpty(checkbox.ElementAction))
        {
            htmxAttrs = $" hx-post=\"{htmlEncode(checkbox.ElementAction)}\"";

            if (!string.IsNullOrEmpty(checkbox.ElementTarget))
                htmxAttrs += $" hx-target=\"{htmlEncode(checkbox.ElementTarget)}\"";

            var swapValue = checkbox.ElementSwap switch
            {
                SwapStrategy.InnerHTML => "innerHTML",
                SwapStrategy.OuterHTML => "outerHTML",
                SwapStrategy.BeforeBegin => "beforebegin",
                SwapStrategy.AfterBegin => "afterbegin",
                SwapStrategy.BeforeEnd => "beforeend",
                SwapStrategy.AfterEnd => "afterend",
                SwapStrategy.None => "none",
                _ => "innerHTML"
            };
            htmxAttrs += $" hx-swap=\"{swapValue}\"";

            // Always include hx-vals for ModelId
            var hxValsContent = checkbox.ElementModelId.HasValue
                ? $"{{\"id\": {checkbox.ElementModelId.Value}}}"
                : "{\"_\":\"\"}";
            htmxAttrs += $" hx-vals='{hxValsContent}'";

            // Trigger on change for checkboxes
            htmxAttrs += " hx-trigger=\"change\"";
        }

        sb.Append($"<input type=\"checkbox\" class=\"checkbox\"{idAttr}{nameAttr}{checkedAttr}{valueAttr}{htmxAttrs}{disabledAttr} />");
    }

    private void renderAlert(Alert alert, StringBuilder sb)
    {
        var toneClass = alert.ElementTone switch
        {
            AlertTone.Neutral => "alert--neutral",
            AlertTone.Positive => "alert--positive",
            AlertTone.Warning => "alert--warning",
            AlertTone.Critical => "alert--critical",
            _ => "alert--neutral"
        };

        var classes = $"alert {toneClass}";
        var idAttr = !string.IsNullOrEmpty(alert.ElementId) ? $" id=\"{htmlEncode(alert.ElementId)}\"" : "";
        var roleAttr = " role=\"alert\"";

        sb.Append($"<div class=\"{classes}\"{idAttr}{roleAttr}>");
        sb.Append("<p class=\"alert__message\">");
        sb.Append(htmlEncode(alert.Message));
        sb.Append("</p>");
        sb.Append("</div>");
    }

    private void renderBadge(Badge badge, StringBuilder sb)
    {
        var toneClass = badge.ElementTone switch
        {
            BadgeTone.Neutral => "badge--neutral",
            BadgeTone.Accent => "badge--accent",
            BadgeTone.Positive => "badge--positive",
            BadgeTone.Warning => "badge--warning",
            BadgeTone.Critical => "badge--critical",
            _ => "badge--neutral"
        };

        var sizeClass = badge.ElementSize switch
        {
            BadgeSize.Small => "badge--small",
            BadgeSize.Medium => "badge--medium",
            _ => "badge--medium"
        };

        var classes = $"badge {toneClass} {sizeClass}";
        var idAttr = !string.IsNullOrEmpty(badge.ElementId) ? $" id=\"{htmlEncode(badge.ElementId)}\"" : "";

        sb.Append($"<span class=\"{classes}\"{idAttr}>");
        sb.Append(htmlEncode(badge.Label));
        sb.Append("</span>");
    }

    private void renderEmptyState(EmptyState emptyState, StringBuilder sb)
    {
        var sizeClass = emptyState.ElementSize switch
        {
            EmptyStateSize.Small => "empty-state--small",
            EmptyStateSize.Medium => "empty-state--medium",
            EmptyStateSize.Large => "empty-state--large",
            _ => "empty-state--medium"
        };

        var classes = $"empty-state {sizeClass}";

        sb.Append($"<div class=\"{classes}\">");
        sb.Append("<p class=\"empty-state__message\">");
        sb.Append(htmlEncode(emptyState.Message));
        sb.Append("</p>");

        if (!string.IsNullOrEmpty(emptyState.ElementActionLabel) && !string.IsNullOrEmpty(emptyState.ElementActionUrl))
        {
            sb.Append($"<a class=\"empty-state__action\" href=\"{htmlEncode(emptyState.ElementActionUrl)}\">");
            sb.Append(htmlEncode(emptyState.ElementActionLabel));
            sb.Append("</a>");
        }

        sb.Append("</div>");
    }

    private void renderList(List list, StringBuilder sb)
    {
        var styleClass = list.ElementStyle switch
        {
            ListStyle.Unordered => "list--unordered",
            ListStyle.Ordered => "list--ordered",
            ListStyle.Plain => "list--plain",
            _ => "list--unordered"
        };

        var classes = $"list {styleClass}";
        var idAttr = !string.IsNullOrEmpty(list.ElementId) ? $" id=\"{htmlEncode(list.ElementId)}\"" : "";
        var tagName = list.ElementStyle == ListStyle.Ordered ? "ol" : "ul";

        sb.Append($"<{tagName} class=\"{classes}\"{idAttr}>");

        foreach (var child in ((IElement)list).Children)
            renderElement(child, sb);

        sb.Append($"</{tagName}>");
    }

    private void renderListItem(ListItem listItem, StringBuilder sb)
    {
        var stateClass = listItem.ElementState switch
        {
            ListItemState.Normal => "list-item--normal",
            ListItemState.Completed => "list-item--completed",
            ListItemState.Disabled => "list-item--disabled",
            ListItemState.Error => "list-item--error",
            _ => "list-item--normal"
        };

        var classes = $"list-item {stateClass}";
        var idAttr = !string.IsNullOrEmpty(listItem.ElementId) ? $" id=\"{htmlEncode(listItem.ElementId)}\"" : "";
        var draggableAttr = listItem.DragSourceId.HasValue ? " draggable=\"true\"" : "";
        var dragIdAttr = listItem.DragSourceId.HasValue ? $" data-drag-id=\"{listItem.DragSourceId.Value}\"" : "";

        sb.Append($"<li class=\"{classes}\"{idAttr}{draggableAttr}{dragIdAttr}>");

        foreach (var child in ((IElement)listItem).Children)
            renderElement(child, sb);

        sb.Append("</li>");
    }

    private void renderTabs(Tabs tabs, StringBuilder sb)
    {
        var styleClass = tabs.ElementStyle switch
        {
            TabStyle.Underline => "tabs--underline",
            TabStyle.Boxed => "tabs--boxed",
            TabStyle.Pill => "tabs--pill",
            _ => "tabs--underline"
        };

        var classes = $"tabs {styleClass}";
        var idAttr = !string.IsNullOrEmpty(tabs.ElementId)
            ? $" id=\"{htmlEncode(tabs.ElementId)}\""
            : "";

        sb.Append($"<div class=\"{classes}\"{idAttr}>");

        // Render triggers container
        sb.Append("<div class=\"tabs__triggers\" role=\"tablist\">");
        foreach (var tab in tabs._tabs)
        {
            renderTabTrigger(tab, tabs.ElementId, sb);
        }
        sb.Append("</div>");

        // Render panels container
        sb.Append("<div class=\"tabs__panels\">");
        var hasHtmxAction = tabs._tabs.Any(t => !string.IsNullOrEmpty(t.ElementAction));
        foreach (var tab in tabs._tabs)
        {
            // In HTMX mode, only render active panel to reduce payload
            if (hasHtmxAction && !tab.ElementActive)
                continue;

            renderTabPanel(tab, sb);
        }
        sb.Append("</div>");

        sb.Append("</div>");
    }

    private void renderTabTrigger(Tab tab, string tabsId, StringBuilder sb)
    {
        var classes = tab.ElementActive
            ? "tabs__trigger tabs__trigger--active"
            : "tabs__trigger";

        var ariaSelected = tab.ElementActive ? "true" : "false";
        var panelId = !string.IsNullOrEmpty(tab.ElementId)
            ? $"tab-panel-{tab.ElementId}"
            : "";

        var ariaControls = !string.IsNullOrEmpty(panelId)
            ? $" aria-controls=\"{panelId}\""
            : "";

        // Build HTMX attributes if Action is specified
        var htmxAttrs = "";
        if (!string.IsNullOrEmpty(tab.ElementAction))
        {
            htmxAttrs = $" hx-get=\"{htmlEncode(tab.ElementAction)}\"";

            var target = !string.IsNullOrEmpty(tab.ElementTarget)
                ? tab.ElementTarget
                : (!string.IsNullOrEmpty(tabsId) ? $"#{tabsId}" : "");
            if (!string.IsNullOrEmpty(target))
                htmxAttrs += $" hx-target=\"{htmlEncode(target)}\"";

            var swapValue = tab.ElementSwap switch
            {
                SwapStrategy.InnerHTML => "innerHTML",
                SwapStrategy.OuterHTML => "outerHTML",
                SwapStrategy.BeforeBegin => "beforebegin",
                SwapStrategy.AfterBegin => "afterbegin",
                SwapStrategy.BeforeEnd => "beforeend",
                SwapStrategy.AfterEnd => "afterend",
                SwapStrategy.None => "none",
                _ => "outerHTML"
            };
            htmxAttrs += $" hx-swap=\"{swapValue}\"";
        }

        sb.Append($"<button class=\"{classes}\" role=\"tab\" aria-selected=\"{ariaSelected}\"{ariaControls}{htmxAttrs}>");
        sb.Append(htmlEncode(tab.Label));
        sb.Append("</button>");
    }

    private void renderTabPanel(Tab tab, StringBuilder sb)
    {
        var classes = tab.ElementActive
            ? "tabs__panel tabs__panel--active"
            : "tabs__panel";

        var panelId = !string.IsNullOrEmpty(tab.ElementId)
            ? $" id=\"tab-panel-{tab.ElementId}\""
            : "";

        var hiddenAttr = tab.ElementActive ? "" : " hidden";

        sb.Append($"<div class=\"{classes}\"{panelId} role=\"tabpanel\"{hiddenAttr}>");

        if (tab._content != null)
        {
            foreach (var child in tab._content.Children)
                renderElement(child, sb);
        }

        sb.Append("</div>");
    }

    private void renderTopNav(TopNav topNav, StringBuilder sb)
    {
        var idAttr = !string.IsNullOrEmpty(topNav.ElementId)
            ? $" id=\"{htmlEncode(topNav.ElementId)}\""
            : "";

        sb.Append($"<header class=\"top-nav\"{idAttr}>");

        // Inner container (max-width constrained)
        sb.Append("<div class=\"top-nav__inner\">");

        // Logo (left side)
        if (!string.IsNullOrEmpty(topNav.ElementLogoText))
        {
            var href = !string.IsNullOrEmpty(topNav.ElementLogoHref)
                ? topNav.ElementLogoHref
                : "/";
            sb.Append($"<a class=\"top-nav__logo\" href=\"{htmlEncode(href)}\">");
            sb.Append(htmlEncode(topNav.ElementLogoText));
            sb.Append("</a>");
        }

        // Actions slot (right side)
        if (topNav._actionsSlot != null && topNav._actionsSlot.Children.Any())
        {
            sb.Append("<div class=\"top-nav__actions\">");
            foreach (var child in topNav._actionsSlot.Children)
                renderElement(child, sb);
            sb.Append("</div>");
        }

        sb.Append("</div>"); // Close inner
        sb.Append("</header>");
    }

    private void renderLink(Link link, StringBuilder sb)
    {
        var styleClass = link.ElementStyle switch
        {
            LinkStyle.Default => "link",
            LinkStyle.Button => "link link--button",
            LinkStyle.ButtonSecondary => "link link--button-secondary",
            LinkStyle.Ghost => "link link--ghost",
            _ => "link"
        };

        var idAttr = !string.IsNullOrEmpty(link.ElementId)
            ? $" id=\"{htmlEncode(link.ElementId)}\""
            : "";

        sb.Append($"<a class=\"{styleClass}\" href=\"{htmlEncode(link.ElementHref)}\"{idAttr}>");
        sb.Append(htmlEncode(link.Label));
        sb.Append("</a>");
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
