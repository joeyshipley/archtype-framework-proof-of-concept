# ArchType Framework Proof-of-Concept

ArchType reduces web development complexity by encoding solved technical decisions as defaults—not documentation. Through server-authoritative architecture and a closed-world UI system, it enforces a strict separation between framework infrastructure and feature development. Developers write business logic using consistent patterns—no CSS, no JavaScript, no HTML templates, no security configuration. 

The trade-off is flexibility: this framework targets the 70% of web applications that don't need SPAs or real-time streaming. For our web applications, a junior developer or AI assistant can ship features that would require senior-level knowledge in mainstream frameworks.

**[Development Setup](./README_SETUP.md)** - Prerequisites, installation, and common tasks.

**License:** [MIT](./LICENSE)

## Project Structure

*NOTE: App name changed and is no longer PagePlay. If you see that, just ignore.*

```
PagePlay/
├── PagePlay.Site/          # Main web application
│   ├── Application/        # Feature vertical slices
│   ├── Pages/              # Page views and interactions
│   └── Infrastructure/     # Framework, data, UI rendering
├── PagePlay.Tests/         # Test project
└── docker-compose.yml      # PostgreSQL container config
```

See `.claude/docs/` for detailed architecture documentation.

## Framework Features

### CSS Abstraction (Closed-World UI)

Developers declare semantic intent using a finite vocabulary of UI elements instead of writing CSS. This ensures visual consistency, enables designer-controlled theming, and prevents invalid styling combinations at compile time.

**Developers write:** `Button(Importance.Primary)`, `Stack(For.Fields)`, `Grid(For.Cards, Columns.Three)`
**Framework handles:** CSS generation, responsive behavior, theming, spacing tokens, context-aware styling

### JavaScript/HTMX Abstraction

Interactions are declared as properties on elements rather than written as JavaScript code. This keeps the client thin (8-10KB), maintains server authority over all state, and eliminates an entire category of client-side bugs. This doesn't sacrifice rich UI—pure client-side interactions like drag-and-drop work seamlessly alongside server interactions (see `wwwroot/js/drag-drop.js` for an example).

**Developers write:** `.Action("/endpoint")`, `.ModelId(id)`, `DataMutations.For("domain")`
**Framework handles:** All JS, DOM manipulation, AJAX requests, OOB swaps, hx-* attribute injection
**Current JS:** 926 bytes gzipped (2.2 KB minified) — see `wwwroot/js/archtype.min.js`
**Estimated complete framework JS:** ~17 KB raw → ~10 KB minified → ~5 KB gzipped

### Data Loading Abstraction

Views declare what data they need via type-safe dependencies, and the framework fetches it in parallel before rendering. This eliminates N+1 queries, prevents data fetching in render methods, and ensures views are pure functions.

**Developers write:** `DataDependencies.From<T>()`, `data.Get<T>()`, Provider classes
**Framework handles:** Parallel fetching, type-safe access, automatic re-fetching on mutations

### Rendering Abstraction

UI is built as C# element trees that the framework converts to HTML with automatic encoding. This prevents XSS vulnerabilities, eliminates template syntax errors, and enables compile-time validation of UI structure.

**Developers write:** Element trees in C# (`new Page(new Section(...))`)
**Framework handles:** HTML generation, entity encoding, metadata injection, view lifecycle

### Testing Abstraction

Tests inherit from a base class that automatically sets up DI and mocking infrastructure. This eliminates boilerplate, ensures consistent test patterns, and lets developers focus on test logic rather than setup.

**Developers write:** `class Tests : SetupTestFor<T>`, use `SUT` and `Mocker`
**Framework handles:** DI container setup, auto-mocking, service provider configuration, fake injection

### Form Handling Abstraction

Forms are built with semantic elements and validated via FluentValidation contracts. This centralizes validation rules, automatically displays field errors, and ensures consistent form behavior across the application.

**Developers write:** `Form().Action()`, `Input().Name()`, `AbstractValidator<T>`
**Framework handles:** Model binding, validation execution, error display, HTMX form submission

### Database Abstraction

Queries are expressed as reusable Specifications using LINQ expressions rather than SQL strings. This prevents SQL injection, enables query composition, and keeps queries co-located with domain models.

**Developers write:** `Specification<T>(t => t.Id == id)`, `_repository.Get<T>(spec)`
**Framework handles:** Parameterized SQL generation, includes, transactions, connection management

### Routing Abstraction

Interactions inherit from a base class that handles performer execution, authentication, and response orchestration. This standardizes endpoint patterns, automates OOB updates, and separates success/error handling clearly.

**Developers write:** `RouteBase`, `RouteAction`, `OnSuccess()`, `OnError()`
**Framework handles:** Endpoint registration, auth enforcement, form binding, performer execution, OOB orchestration

### Caching Architecture (Not Yet Implemented)

Server-side rendering with database-backed state means every request hits the server and potentially the database. Without client-side state to absorb repeated reads, external caching infrastructure becomes essential at scale. Redis or similar distributed caching will be needed to cache rendered HTML fragments, query results, and session data.

**Will need:** Redis/Memcached integration, cache invalidation on mutations, cache-aside pattern in providers
**Trade-off:** Architectural simplicity (no client state sync) is exchanged for operational complexity (cache infrastructure)

## Security Features

### Framework-Level Security

- Server authority: client cannot manipulate state, all validation server-side
- Thin client: minimal attack surface, no client-side business logic
- Scoped user context: `ICurrentUserContext` is request-scoped, prevents leakage
- Middleware order: HTTPS → Compression → Logging → Rate Limit → Security Headers → Auth → Anti-forgery

### Rate Limiting & DoS Protection

- 250 requests/minute per user/IP with sliding window algorithm
- Returns 429 Too Many Requests with rate limit headers
- Request size limits: Body 5MB, Headers 64KB, Line 16KB, Timeout 30s

### Security Headers

- `X-Content-Type-Options: nosniff` (MIME sniffing prevention)
- `X-Frame-Options: SAMEORIGIN` (clickjacking protection)
- `Referrer-Policy: strict-origin-when-cross-origin` (referrer leakage prevention)
- `Content-Security-Policy` with strict policy (XSS & injection prevention)
- `Strict-Transport-Security` with 1 year max-age (force HTTPS in production)

### CSRF Protection

- Anti-forgery tokens with custom `X-XSRF-TOKEN` header for HTMX
- Token provider for forms via `GetAndStoreTokens()`

### XSS Prevention

- Strict Content Security Policy (`default-src 'self'`, limited script sources)
- Server-side rendering with encoding (no client-side templating)

### Authentication & Authorization

- JWT token generation with HMAC-SHA256 signing, zero clock skew, unique Jti claims
- Full JWT validation (issuer, audience, lifetime, signing key)
- HTTP cookie fallback for browser/HTMX requests
- Auth filter that enforces authentication and populates user context
- Resource-based authorization via `IsOwnedBy()` ownership checks in performers

### Data Protection

- Password hashing with BCrypt (work factor 12) + HMAC-SHA256 pepper
- Secure cookies: HttpOnly, Secure, SameSite=Strict, 60 minute expiry
- Configuration validation on startup (JWT secret 32+ chars, pepper required)

### Input Validation & Sanitization

- FluentValidation on all request contracts (email format, password rules, required fields)
- HTML encoding on all user content (escapes `& < > " '`)
- `Safe()` helper using `HttpUtility.HtmlEncode()`
- Parameterized queries via Specification pattern with LINQ/EF Core (no SQL injection)

### Error Handling

- Development: detailed exception page
- Production: generic error page (no stack traces)
- User-friendly validation error messages
- Request logging with correlation IDs (no body logging)

## Planned Rich Client Features

### Server-Side Only (HTMX handles it, no custom JS needed)

- Tabs - `hx-get` on tab click, swap panel content — **0 lines, 0 bytes**
- Accordion - `hx-get` toggles panel, server returns expanded/collapsed state — **0 lines, 0 bytes**
- Infinite Scroll - `hx-trigger="revealed"` on sentinel element — **0 lines, 0 bytes**
- Autocomplete - `hx-trigger="keyup changed delay:300ms"` with server filtering — **0 lines, 0 bytes**
- Modal content - `hx-get` loads modal body into container — **0 lines, 0 bytes**

### Hybrid (server state + client JS for UX polish)

- Modal - focus trap, escape key, click-outside close — **~60 lines, ~1.8 KB**
- Dropdown - click-outside close, keyboard navigation — **~45 lines, ~1.4 KB**
- Toast - auto-dismiss timer, stacking, pause on hover — **~50 lines, ~1.5 KB**
- Sortable - drag reordering visuals within list — **~70 lines, ~2.2 KB**
- Autosave - debouncing, dirty state indicator — **~35 lines, ~1.0 KB**

**Hybrid subtotal: ~260 lines, ~7.9 KB (~2.5 KB gzipped)**

### Client-Side Only (browser APIs / zero latency required)

- Clipboard - async clipboard API wrapper — **~15 lines, ~450 bytes**
- Tooltips - positioning, hover delay, viewport collision — **~65 lines, ~2.0 KB**
- Keyboard shortcuts - global hotkey registry — **~40 lines, ~1.2 KB**
- Resize panels - frame-by-frame drag feedback — **~60 lines, ~1.8 KB**

**Client-only subtotal: ~180 lines, ~5.5 KB (~1.7 KB gzipped)**

### Estimated Totals

| Category | Lines | Raw | Gzipped |
|----------|-------|-----|---------|
| Current (drag-drop, csrf, context, config) | 115 | 3.9 KB | 926 bytes |
| Hybrid features | ~260 | ~7.9 KB | ~2.5 KB |
| Client-only features | ~180 | ~5.5 KB | ~1.7 KB |
| **Total framework JS** | **~555** | **~17.3 KB** | **~5.1 KB** |

## 1.0 Roadmap
- Self-teaching documentation (code examples, errors that reference docs, pattern guides—the framework teaches you how to use it rather than requiring external knowledge)
- CORS configuration (currently defaults to same-origin)
- Page testing practices and infrastructure.
- Continue building out the rich clientside behaviors.
- Caching architecture (Redis integration, cache invalidation strategy)

## 1.x Roadmap
- UX patterns as architectural constraints (server-side pagination, sensible list limits, filtering over scrolling—prevent bad UX the same way we prevent bad CSS)
- Closed-world deployment (IaC generated from app dependencies, not hand-crafted—the framework knows what it needs)
- Closed-world observability (logging, metrics, events with standardized patterns—where it exports is configuration, not code)
- Closed-world feature flags (standardized API for rollouts and toggles—provider is configuration, not code)

