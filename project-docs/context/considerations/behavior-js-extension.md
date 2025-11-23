# Behavior.js Extension - UX Enhancement Layer

**Status:** Consideration for Future Implementation
**Created:** 2025-11-23
**Context:** Server-authoritative architecture with HTTP-First approach

---

## Purpose

A minimal JavaScript layer (~5KB gzipped) that bridges the gap between pure server-side rendering and SPA-like user experience without compromising our server authority model.

**Core principle:** Enhance perceived responsiveness while maintaining server as source of truth.

---

## What It Gives Us

### **1. Debounced Inputs (Autocomplete, Search)**

**Problem:** Every keystroke = 150ms server round-trip = janky UX

**Solution:**
```javascript
// Wait 300ms after user stops typing before hitting server
export function debounce(func, wait) {
  let timeout;
  return function(...args) {
    clearTimeout(timeout);
    timeout = setTimeout(() => func.apply(this, args), wait);
  };
}
```

**Usage:**
```html
<input
  hx-post="/search"
  hx-trigger="keyup changed delay:300ms"
  hx-target="#results"
/>
```

**Benefit:** Reduces requests from 6-10 per search to 1, feels responsive instead of laggy.

**Applies to:**
- Search-as-you-type
- Autocomplete
- Live filters
- Any rapid input scenario

---

### **2. Optimistic UI Updates**

**Problem:** Toggle checkbox → wait 150ms → see change = feels slow

**Solution:**
```javascript
// Update UI immediately, server sync in background
export function optimisticToggle(element) {
  element.addEventListener('change', () => {
    // Visual update is instant
    element.disabled = true; // Prevent double-clicks

    // HTMX syncs in background
    // If server rejects, HTMX morphs back to correct state
  });
}
```

**Benefit:** Checkboxes, toggles, buttons feel instant (< 100ms) while server maintains authority.

**Applies to:**
- Todo completion toggles
- Feature flags
- Like/favorite buttons
- Any boolean state toggle

---

### **3. Client-Side Filtering (Progressive Enhancement)**

**Problem:** Server fetch for every filter change = slow, excessive requests

**Solution:**
```javascript
// Filter already-loaded results instantly, fetch fresh data in background
export function clientSideFilter(input, items) {
  input.addEventListener('input', (e) => {
    const query = e.target.value.toLowerCase();
    items.forEach(item => {
      item.style.display = item.textContent.toLowerCase().includes(query)
        ? ''
        : 'none';
    });

    // HTMX fetches authoritative results after debounce
  });
}
```

**Benefit:** Instant feedback on visible results, server provides fresh data when ready.

**Applies to:**
- Table filtering
- Tag filtering
- Category selection
- Any list filtering

---

### **4. Character Counters**

**Problem:** Character limit feedback needs instant response

**Solution:**
```javascript
// Count locally, server validates on submit
export function characterCounter(input, counter, max) {
  input.addEventListener('input', (e) => {
    const remaining = max - e.target.value.length;
    counter.textContent = `${remaining} characters remaining`;
    counter.classList.toggle('warning', remaining < 20);
  });
}
```

**Benefit:** Real-time feedback without server round-trips.

**Applies to:**
- Text inputs with limits (titles, descriptions)
- Tweet-style character limits
- Bio fields
- Any constrained text input

---

### **5. Confirm Dialogs**

**Problem:** Need to prevent accidental deletions before server sees request

**Solution:**
```javascript
// Confirm before HTMX sends request
export function confirmAction(button, message) {
  button.addEventListener('click', (e) => {
    if (!confirm(message)) {
      e.preventDefault(); // Stop HTMX request
    }
  });
}
```

**Benefit:** User-friendly confirmation without custom modal HTML.

**Applies to:**
- Delete actions
- Irreversible operations
- Logout confirmations
- Destructive actions

---

### **6. Form Validation Feedback**

**Problem:** Wait for server to show validation errors = feels unresponsive

**Solution:**
```javascript
// Basic client-side validation for instant feedback
export function validateRequired(input) {
  input.addEventListener('blur', () => {
    if (!input.value.trim()) {
      input.classList.add('invalid');
      showError(input, 'This field is required');
    } else {
      input.classList.remove('invalid');
      hideError(input);
    }

    // Server still validates on submit (source of truth)
  });
}
```

**Benefit:** Instant feedback on obvious errors, server validates thoroughly.

**Applies to:**
- Required fields
- Email format
- Number ranges
- Simple validation rules

---

### **7. Loading States**

**Problem:** No visual feedback during server round-trip

**Solution:**
```javascript
// Show spinner during HTMX requests
export function loadingIndicator(element) {
  document.body.addEventListener('htmx:beforeRequest', () => {
    element.classList.add('loading');
  });

  document.body.addEventListener('htmx:afterRequest', () => {
    element.classList.remove('loading');
  });
}
```

**Benefit:** User knows something is happening during 150ms wait.

**Applies to:**
- Forms
- Buttons
- Search inputs
- Any async action

---

### **8. Keyboard Shortcuts**

**Problem:** Power users want keyboard navigation

**Solution:**
```javascript
// Add common shortcuts without complex state management
export function keyboardShortcuts(shortcuts) {
  document.addEventListener('keydown', (e) => {
    const key = `${e.ctrlKey ? 'Ctrl+' : ''}${e.key}`;
    if (shortcuts[key]) {
      e.preventDefault();
      shortcuts[key]();
    }
  });
}
```

**Usage:**
```javascript
keyboardShortcuts({
  'Ctrl+k': () => document.getElementById('search').focus(),
  '/': () => document.getElementById('search').focus(),
  'Escape': () => closeModal()
});
```

**Benefit:** Professional UX without SPA framework.

**Applies to:**
- Search focus (/)
- Modal close (Esc)
- Navigation shortcuts
- Power user features

---

## Implementation Strategy

### **File Structure**
```
wwwroot/
  js/
    behavior.js         (~5KB gzipped - core utilities)
    behavior.todo.js    (~1KB - todo-specific behaviors)
    behavior.search.js  (~1KB - search-specific behaviors)
```

### **Usage Pattern**
```html
<!-- Load core behaviors -->
<script src="/js/behavior.js" type="module"></script>

<!-- Apply to specific elements -->
<script type="module">
  import { debounce, optimisticToggle } from '/js/behavior.js';

  // Apply behaviors declaratively
  document.querySelectorAll('[data-debounce]').forEach(el => {
    debounce(el, parseInt(el.dataset.debounce));
  });

  document.querySelectorAll('[data-optimistic]').forEach(el => {
    optimisticToggle(el);
  });
</script>
```

### **Progressive Enhancement**
- Works with JavaScript disabled (HTMX still functional)
- Enhances experience when JavaScript enabled
- No build step required
- Native ES modules (modern browsers only)

---

## What We Get

### **Perceived Performance Improvements**
- Autocomplete: 900ms → 400ms (feels responsive)
- Toggles: 150ms → < 50ms (feels instant)
- Filters: 150ms per keystroke → instant visual feedback
- Validation: 150ms → instant for basic rules
- Character counters: 150ms → instant

### **Reduced Server Load**
- Search requests: 10x reduction (debouncing)
- Filter requests: 5x reduction (client-side filtering)
- Validation requests: Eliminated for basic checks

### **Better User Experience**
- Instant feedback for obvious interactions
- Loading states show progress
- Keyboard shortcuts for power users
- Confirms prevent mistakes

### **Still Maintains Server Authority**
- Server validates all submissions
- Server controls all routing/navigation
- Server owns all business logic
- Server is source of truth for data

---

## What We Still Can't Do

**Be honest about limitations - recommend the right tool for these:**

### **Not Suitable For:**
1. **Real-time collaboration** (Google Docs-style) - Use dedicated real-time framework
2. **Canvas/drawing applications** (60fps required) - Use SPA with canvas library
3. **Offline-first apps** - Use SPA with service workers/IndexedDB
4. **Complex spreadsheets** - Use specialized spreadsheet library
5. **Drag-and-drop builders** - Use SPA for smooth dragging feedback

### **Architectural Trade-off:**
We optimize for server authority over instant everything. The 150ms round-trip is a feature (server validates), not a bug. For the 90% of apps that are CRUD/forms/dashboards, this is the right trade-off.

---

## Decision Criteria

### **When to Add a Behavior:**
- ✅ Solves common UX problem across multiple features
- ✅ < 500 bytes gzipped when added to bundle
- ✅ Works as progressive enhancement (graceful degradation)
- ✅ Doesn't duplicate server logic (client shows, server validates)
- ✅ Improves perceived performance by > 100ms

### **When NOT to Add a Behavior:**
- ❌ Duplicates server logic (creates drift risk)
- ❌ Only used once (just inline it)
- ❌ Requires large library (> 5KB)
- ❌ Requires complex state management
- ❌ Better solved by different architecture (SPA for that feature)

---

## Bundle Size Budget

**Target:** 5-8KB gzipped for core behaviors
**Maximum:** 10KB gzipped including all extensions

**For comparison:**
- React + ReactDOM: ~140KB gzipped
- Vue 3: ~35KB gzipped
- Our entire client bundle (HTMX + idiomorph + behavior.js): ~12KB gzipped

**We can add 50+ behaviors before matching Vue's size.**

---

## Next Steps

1. **Phase 1:** Implement core behaviors (debounce, optimistic, confirm)
2. **Phase 2:** Add character counter and loading indicators
3. **Phase 3:** Add keyboard shortcuts and client-side filtering
4. **Phase 4:** Profile and optimize bundle size
5. **Phase 5:** Document patterns for future behaviors

**Don't build until needed.** Wait for real UX pain points, then add targeted solutions.

---

## References

- Phoenix LiveView uses similar patterns (~3KB client)
- Laravel Livewire has Alpine.js integration (~15KB)
- Hotwire uses Stimulus (~30KB framework)

**Our approach:** Cherry-pick the best patterns, skip the framework overhead.

---

**Last Updated:** 2025-11-23
**Status:** Consideration - not yet implemented
