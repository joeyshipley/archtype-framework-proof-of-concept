# Design Decision: Tab Interactivity Approach

**Status:** Open Question
**Created:** 2026-01-10
**Related:** flowbite-theme-implementation.md (Task 2.1 - Tabs/Tab)

---

## Context

We've implemented `Tabs` and `Tab` vocabulary elements with three style variants (Underline, Boxed, Pill). The C# implementation includes HTMX support (`.Action()`, `.Target()`, `.Swap()`) but currently tabs don't switch interactively - users see only the active tab's content.

PagePlay intentionally has no JavaScript. Rich interactions come from HTMX and server-side rendering. This is the first component where we need to decide how to handle client-side interactivity.

---

## The Question

**How should tab switching work?**

1. **Server-side (HTMX)** - Click triggers HTTP request, server returns new tab state
2. **Client-side (JavaScript)** - Tiny JS file toggles visibility of pre-rendered panels
3. **Hybrid** - Server-side by default, JS enhancement for static/lightweight cases

---

## Option 1: Server-Side (HTMX)

All tab switches go through the server.

**Implementation:**
```csharp
new Tabs()
    .Id("product-tabs")
    .Tab(new Tab("Details")
        .Active(activeTab == "details")
        .Action("/products/123/tab/details"))
    .Tab(new Tab("Reviews")
        .Active(activeTab == "reviews")
        .Action("/products/123/tab/reviews"))
```

**Server endpoint:**
```csharp
app.MapGet("/products/{id}/tab/{tabId}", (long id, string tabId) =>
{
    // Return entire Tabs component with new active tab
    var tabs = BuildProductTabs(id, activeTab: tabId);
    return Results.Content(renderer.Render(tabs), "text/html");
});
```

**Pros:**
- Already built into Tab implementation
- Only loads active content (efficient for heavy/data-rich tabs)
- Consistent with PagePlay's HTTP-first, server-authoritative philosophy
- Same pattern as every other interaction in the app
- Server remains single source of truth
- Easy to add logging, auth checks, analytics per tab view

**Cons:**
- Network latency on every tab switch (~50-200ms)
- Requires endpoint per tab context (or parameterized)
- Feels less "instant" for lightweight tabs

---

## Option 2: Client-Side (JavaScript)

Tiny JS framework file handles tab switching.

**Implementation:**
- Render ALL tab panels upfront (with `hidden` attribute on inactive)
- JS listens for clicks on `.tabs__trigger`
- Toggles `hidden` and `--active` classes
- No server interaction

**Pros:**
- Instant switching (no network latency)
- No server endpoints needed for static tabs
- Better UX for lightweight content

**Cons:**
- Renders ALL content upfront (wasteful for heavy tabs)
- Introduces new pattern (JS) to the codebase
- State lives in two places (client visibility vs server truth)
- Need to maintain JS file
- Diverges from PagePlay's server-authoritative model

---

## Option 3: Hybrid (Recommended)

Server-side by default, with progressive JS enhancement for static cases.

**Behavior:**
- If Tab has `.Action()` configured → HTMX handles it (server-side)
- If Tab has NO action → tiny JS toggles pre-rendered panels (client-side)

**Implementation:**
- ~20 lines of vanilla JS
- Only activates for tabs without `hx-get` attributes
- Gracefully degrades (no JS = just shows active tab)

**Pros:**
- Production tabs: Server-side, data loaded on demand
- Showcase/lightweight tabs: Instant client-side switching
- No JS scenario: Still works, shows first tab
- Data authority stays server-side
- JS is purely visibility toggle, not state management

**Cons:**
- Two code paths to understand
- Need to create/maintain JS file

---

## PagePlay Philosophy Considerations

**HTTP-first, server-authoritative:**
> "Treating web apps like turn-based games"

Server-side tabs align with this. Each tab click is a "turn" - user makes a request, server responds with new state.

**Consistent complexity:**
> "Every feature should feel similar in structure and effort"

Server-side tabs follow the same pattern as forms, buttons, checkboxes - click triggers server action, OOB updates render result.

**Closed-World UI:**
> "Developers declare WHAT, designers control HOW"

Both approaches maintain this - appearance is theme-controlled regardless of interactivity mechanism.

---

## Current State

- Tabs vocabulary: ✅ Complete
- HTML rendering: ✅ Complete (includes ARIA, hidden attribute)
- CSS styling: ✅ Complete (three variants)
- HTMX support in C#: ✅ Built in (Action/Target/Swap)
- Server endpoints: ❌ Not implemented
- JavaScript enhancement: ❌ Not implemented
- Showcase tabs: ⚠️ Static (only shows active tab content)

---

## Open Questions

1. **Which approach should be the primary pattern?**
   - Leaning: Server-side (HTMX) for consistency

2. **Should we implement JS enhancement for showcase/lightweight cases?**
   - Leaning: Yes, but as progressive enhancement only

3. **Where should the JS file live?**
   - Options: `wwwroot/js/closed-world.js` or `wwwroot/js/tabs.js`

4. **Should the JS be auto-loaded or opt-in?**
   - Auto-load in layout vs `<script>` tag when needed

---

## Next Steps

*To be determined after decision is made.*

1. [ ] Decide on primary approach
2. [ ] Implement server-side tab switching (if chosen)
3. [ ] Implement JS enhancement (if chosen)
4. [ ] Update showcase to demonstrate interactive tabs
5. [ ] Document the pattern for future components

---

**Last Updated:** 2026-01-10
