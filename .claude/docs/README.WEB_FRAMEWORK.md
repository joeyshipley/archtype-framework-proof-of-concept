# HTTP-First Philosophy

**Version:** 1.0
**Last Updated:** 2025-11-08
**Status:** Foundation Document

---

## Core Insight

**Web applications ARE turn-based games.**

The game industry solved this problem 20 years ago: rich, responsive client experiences with complete server authority over unreliable networks. We're applying proven game architecture to web development.

**HTTP-First is Phase 1:** Validate the turn-based architecture (server authority, HTML diffing, component model) with the simplest transport before adding WebSocket complexity.

---

## Why HTTP-First?

> **The fastest way to validate an idea is to build the simplest version that tests the core hypothesis.**

Before investing months in WebSocket infrastructure (3,200 LOC), we must prove that:
1. **Server authority** provides good UX with 100-200ms latency
2. **HTML diffing** is fast enough (<5ms) and bandwidth-efficient
3. **Component model** is intuitive for developers
4. **Data pre-fetching** prevents N+1 queries

If the foundation fails, WebSocket was wasted effort. If it succeeds, adding WebSocket becomes straightforward.

**HTTP-First is not a compromise. It's a validation strategy.**

---

## Turn-Based Architecture with HTTP

**Web apps are turn-based games:**
```
E-commerce checkout = Turn-based game
├─ Player action: "Add to cart" (HTTP POST)
├─ Server validates: "In stock? Allowed?"
├─ Server updates state: Cart += item
├─ Server sends result: HTML diff (cart badge)
└─ Player sees update: Cart badge shows 3

This IS a turn-based game. HTTP is sufficient transport.
```

**Every web interaction is a turn:**
- User takes action (click button → HTTP POST)
- Server processes (game logic, validation)
- Server returns result (HTML diff)
- User sees result (DOM update)
- User takes next action (next turn)

**This isn't a metaphor. This is the actual architecture.**

---

## The Pragmatic Path

**Big Bang Approach (Backwards):**
```
Design and build complete WebSocket infrastructure
Discover core component model doesn't work
Realize WebSocket work was premature
```

**HTTP-First Approach:**
```
Build HTTP-only component model
Test with real TODO app
Validate: server authority, HTML diffing, developer experience

If YES → Add WebSocket (Phase 2)
If NO  → Pivot without wasting effort
```

**Comparison:** Time to learning (fast vs slow) • Sunk cost (40% vs 100% LOC) • Risk (low vs high)

---

## Game Patterns Applied to HTTP

### Pattern 1: Server Authority (Non-Negotiable)

```
CLIENT (6-8KB):
├─ Receives HTML diffs from server
├─ Applies CSS styling
├─ Captures events
└─ Sends events only (not state)

SERVER (Any Language - Node/Python/.NET/Go):
├─ Maintains component state
├─ Processes events
├─ Validates business logic
├─ Renders HTML
└─ Sends HTML diffs
```

**Key insight:** Client is rich in presentation, thin in authority. Server owns all truth.

### Pattern 2: Delta Compression (HTML Diffing)

```
Old HTML: <ul id="todos"><li>Todo 1</li></ul>
New HTML: <ul id="todos"><li>Todo 1</li><li>Todo 2</li></ul>

Diff: {"op": "append", "path": "#todos", "value": "<li>Todo 2</li>"}

Bandwidth: 80 bytes vs 2KB full HTML
```

**Game inspiration:** Source Engine's delta compression (send only what changed).

### Pattern 3: Data Pre-Fetching (Game Level Loading)

```
Game level loading:
├─ Discover all entities in level
├─ Load ALL assets in parallel (loading screen)
├─ Instantiate entities with loaded assets
└─ Start gameplay (everything ready, no stuttering)

Component data loading:
├─ Declare data requirements (DataManifest)
├─ Execute queries in parallel (framework handles)
├─ Mount component with data ready
└─ Render (no N+1 queries, no waterfalls)
```

**Result:** Clean "loading → interactive" transition, predictable performance.

#### Data Pre-Fetching Implementation

**DataDomains are the single source of read operations:**

```
Page Load:
├─ Collect required domains (from page + components)
├─ DataLoader fetches ALL domains in parallel
├─ No N+1 queries, no waterfalls
├─ Page and components render from shared DataContext
└─ Clean "loading → interactive" transition

User Action (Mutation):
├─ User clicks "Create Todo" → Workflow executes
├─ Workflow validates, mutates data, returns success
├─ Framework identifies affected domains ("todos")
├─ DataLoader re-fetches ONLY affected domains
├─ Framework re-renders affected components (OOB)
└─ UI updates, ready for next action
```

**Key insight:**
- Workflows = user actions that change state (commands)
- DataDomains = data fetching for rendering (queries)
- No "list workflows" - reads happen through DataLoader
- Clear separation following CQRS principles

---

## Philosophical Foundations

**1. Server Authority (Non-Negotiable)**
Server owns all state, logic, validation. Client owns rendering, event capture, visual predictions. Never: client calculates final state.

**2. Multi-Language Support**
Protocol-based architecture works across Node/Python/.NET/Go. Any decision must work across languages. Specification-first, not implementation-first.

**3. Thin Client (8-10KB Target)**
Initial load speed, simplicity, server authority. Client bundle stays tiny (no framework bloat).

**4. Validate Assumptions Early**
Test the riskiest assumptions first with the simplest implementation: Can server authority provide good UX with 100-200ms latency? Is HTML diffing fast enough (<5ms)? Does data pre-fetching prevent N+1 queries? WebSocket doesn't change these answers.

**5. Complexity is a Liability**
Every line of code is a potential bug, maintenance burden, cognitive load, testing overhead, and documentation requirement. WebSocket adds 3,200 LOC before we know if it's needed.

**6. Constraints Force Clarity**
Removing WebSocket forces us to answer: What truly requires real-time (chat, live dashboards) vs what's fine with request/response (CRUD, forms, admin panels)?

**7. Options Are Valuable**
HTTP-first preserves options (add WebSocket later, keep HTTP as default, offer hybrid). WebSocket-first removes options (can't downgrade, complexity baked in).

**8. Architectural Honesty Over Clever Workarounds**
Problems should be obviously painful during development, not hidden behind framework magic. Slow page? Fix the queries or split the page. Too much data? Paginate or reduce. Framework exposes problems early and clearly.

---

## HTTP as First-Class Citizen

HTTP is the primary transport for 70% of web applications. WebSocket is the enhancement for the 30% that need real-time.

**HTTP's Strengths:**
- Universal compatibility (works through every proxy, firewall, CDN)
- Familiar mental model (every developer knows request/response)
- Stateless infrastructure (scale horizontally, no sticky sessions)
- Proven at scale (30+ years, massive tooling ecosystem)

**70% of Apps Don't Need WebSocket:**
Admin panels, CMS, e-commerce, settings pages, forms, user management, invoicing, time tracking. Why force WebSocket complexity on them?

---

## The Simplicity Imperative

> "Simplicity is about subtracting the obvious and adding the meaningful." — John Maeda

HTTP-First removes 2,500 LOC: WebSocket connection management (800), heartbeat logic (200), reconnection with backoff (300), transport fallbacks (400), broadcasting infrastructure (600), session recovery (200).

We keep the meaningful parts: component lifecycle, HTML diffing, data pre-fetching, server authority.

**The Simplicity Test:** Is this essential to validate the core idea? Does it simplify DX? Can we add it later without breaking changes? WebSocket fails test #1—it's not essential to validate server authority, diffing, or component model.

---

## Risk Mitigation Through Incrementalism

**Three Key Risks:**
1. **Technical:** Do core concepts work? (server authority with latency, HTML diffing performance, data pre-fetching)
2. **Developer Experience:** Will developers adopt this? (intuitive model, helpful errors, straightforward debugging)
3. **Market:** Does this solve real problems? (simpler than Hotwire, more ergonomic than HTMX, more accessible than LiveView)

**HTTP-First Reduces Risk:** Test core concepts in weeks (not months), get immediate developer feedback, build reference implementation to validate value proposition before heavy investment.

**The Incremental Approach:**
```
Phase 1: HTTP-only → Validate, Measure, Learn
If successful → Phase 2: Add WebSocket (keep HTTP as default)
If unsuccessful → Pivot based on learnings
```

Small bets, fast feedback, course correction.

---

## Developer Experience First

A fast, scalable, feature-complete framework that developers hate is a failed framework. We optimize for DX first.

**Great DX Means:**
- Familiar concepts (HTTP request/response, components as functions, standard debugging tools)
- Fast feedback loops (change → see result < 5 seconds)
- Progressive disclosure (simple things are simple, complex things possible)
- Clear mental models (component = `state + event → new state + HTML`)

**Mental Model Comparison:**

WebSocket: (1) Establish connection (2) Handle connection events (3) Implement heartbeat (4) Handle reconnection with backoff (5) Handle session recovery (6) Handle message sequencing (7) Handle gap detection (8) Send messages (9) Receive broadcasts (10) Handle connection state in UI

HTTP: (1) User clicks button (2) POST to server (3) Server responds with diff (4) Apply diff to DOM

Which is easier to explain to a junior developer?

---

## The 70% Rule

Most web applications don't need WebSocket. E-commerce admin, user management, settings pages, forms, CRUD apps need 0% real-time. Analytics dashboards need 10%, project management 20%, chat 100%.

**The Complexity Tax:** Every app using a WebSocket framework pays for it (learning WebSocket concepts, infrastructure support, debugging tools, connection error handling) even if they never use real-time features.

**HTTP-First approach:** Default is HTTP (simple, works for 70%). Opt-in to WebSocket (complex, needed by 30%). Don't force 100% of users to pay for features only 30% need.

---

## Boring Technology Wins

> "Choose boring technology. Prefer 5+ years proven in production, used by major companies, low abstraction, web standards."

**HTTP** (1999, 26 years): Billions of servers, every language has libraries, universal tool support.
**WebSocket** (2011, 14 years): Proven but more complex - some firewalls block, varying proxy support.
**PostgreSQL** (1996, 29 years): Battle-tested at scale, proven ACID guarantees, mature tooling.
**Redis** (2009, 16 years): Known performance characteristics, well-understood failure modes.

Boring = fewer production surprises, more Stack Overflow answers, better tooling, clearer best practices.
Exciting = unknown failure modes, less mature tooling, smaller knowledge base, evolving practices.

**The Innovation Budget:**
- **Innovate:** Server authority model, HTML diffing algorithm, data pre-fetching pattern, multi-language protocol
- **Bore:** Transport (HTTP first), database (PostgreSQL), cache (Redis), runtime (.NET/Node/Python)

Innovate where it matters, bore where it doesn't.

**HTTP-First Rationale:** Start with the most boring transport (HTTP) to validate innovations. Add proven-but-complex (WebSocket) only after validation.

---

## Build, Measure, Learn

Traditional: Design → Build → Release → Hope.
HTTP-First: Build → Measure → Learn → Decide.

**Measurements:**
- DX: Time to first feature, junior dev can understand?, component model intuitive (1-10), error messages helpful (1-10)
- Performance: HTTP round trip (<100ms p95?), HTML diff (<5ms p95?), data pre-fetching prevents N+1?
- Value: Simpler than Hotwire?, more ergonomic than HTMX?, multi-language works?

**Go/No-Go Decision:**
✅ GO to WebSocket if ALL: DX ≥ 7/10, performance <200ms p95, multi-language works, simpler than alternatives
❌ PIVOT if ANY: DX < 5/10, performance >500ms p95, multi-language impossible, not simpler

Let metrics guide decisions, not ego.

---

## Constraints Breed Creativity

> "Freedom is the enemy of creativity. Constraints force you to think differently." — Orson Welles

Removing WebSocket forces the question: "How do we provide good UX without server push?" This demands creative solutions—surgical HTML diffing, game-style data pre-fetching, efficient Redis session management, fast round-trip times (<100ms).

**Discoveries:**
- No server push → Most apps poll/refresh manually anyway (GitHub Issues, Jira)
- 100-200ms latency → Fast enough for most interactions (feels responsive)
- No connection state → Simpler = more reliable = better UX for stateless apps
- Standard HTTP cookies → Works everywhere, no special infrastructure

**The Upgrade Path as Feature:** HTTP-only constraint revealed a valuable insight—what if HTTP is the default and WebSocket is opt-in? This is more valuable than WebSocket-only: 70% stay simple, 30% upgrade when needed, same component model, pay complexity cost only where valuable.

The constraint led to a better design.

---

## Upgrade Paths Over Big Bang

**Bad versioning:** v1.0 HTTP-only → v2.0 WebSocket-only (breaking changes, migrate or die)
**Good versioning:** v1.0 HTTP-only → v2.0 HTTP + WebSocket (additive, backwards compatible)

The best migration path is no migration at all.

```csharp
// v1.0 and v2.0: Works without changes
public class TodoListPageComponent : Component { }

// v2.0: Opt-in to WebSocket
[Component(Transport = TransportMode.WebSocket)]
public class LiveDashboardComponent : Component { }
```

**The Hybrid Model:** 90% of app uses HTTP (user management, settings, invoices). 10% uses WebSocket (live dashboard, notifications, chat). Simple by default, complex only where needed, pay for what you use.

---

## What We're NOT Building

**NOT offline-first framework:**
- No Service Workers, no sync engines, no CRDTs
- **Solution:** For offline needs, deploy server locally (Electron/Tauri)

**NOT progressive enhancement framework:**
- JavaScript required for all interactivity
- Without JavaScript: CSS applies (no FOUC), but zero functionality
- **Why:** Server authority via persistent connection requires JS
- **Solution:** For no-JS environments, use traditional frameworks (Rails, Django), not this

**NOT real-time continuous framework:**
- No 60+ updates per second, no client-side physics
- **Solution:** For gaming/whiteboard, use different architecture (game engines)

**NOT SPA framework:**
- No client-side routing/state/virtual DOM
- **Solution:** Server-driven navigation over WebSocket (Phase 2) - smooth like SPA, secure like SSR

**NOT everything framework:**
- No built-in auth providers, error reporting, analytics
- **Solution:** Provide hooks and examples, use ecosystem (Passport.js, Sentry)

---

## What We Reject

**1. Premature Optimization:** "We need WebSocket for maximum performance" → Reality: HTTP is fast enough for 70% of apps, complexity is the enemy of performance

**2. Feature Completeness for v1:** "v1.0 must have every feature" → Reality: v1.0 must validate the core hypothesis, shipping fast beats shipping complete

**3. One Size Fits All:** "Every framework must support every use case" → Reality: HTTP-First is for 70% of apps, chat should use LiveView/Socket.IO, it's OK to have a target audience

**4. Complexity as a Badge of Honor:** "More features = better framework" → Reality: Fewer features done well beats many done poorly, developers choose frameworks that don't get in the way

**5. Technology for Technology's Sake:** "WebSocket is newer/cooler" → Reality: Choose technology that fits the problem, boring technology is often right, spend innovation budget wisely

**6. Coupling Abstraction to Implementation:** "Ship whatever works" → Reality: Specification-first architecture ensures framework survives 10+ years as tech evolves

---

## What We Embrace

**1. Simplicity as a Feature:** The simplest solution that works is the best—easier to understand, debug, maintain, extend

**2. Constraints as Gifts:** Constraints force better thinking—removing WebSocket forced creative solutions, HTTP-only clarified what needs real-time, constraints reveal essence

**3. Incremental Progress:** Small steps, fast feedback, course correction—learn quickly, fail cheaply, iterate rapidly, reduce risk

**4. Developer Empathy:** The developer is the user—happy developers = better apps, simple mental models = fewer bugs, fast feedback = higher productivity, great DX = adoption

**5. Humility:** We don't know if this will work, let's find out quickly—assumptions must be tested, metrics beat opinions, adapt or die

---

## Core Principles (Inherited from Root)

**1. Server Authority (Non-Negotiable)**
Server owns all state, logic, validation. Client owns rendering, event capture, visual predictions.

**2. Thin Client (8-10KB Target)**
Initial load speed, simplicity, server authority. Minimal abstractions, tree-shakeable modules.

**3. Multi-Language Support**
Minimum languages: Node/Python/.NET/Go. Any decision must work across languages. Protocol-based, not implementation-based.

**4. Specification-First Architecture**
Define clear contract boundaries where specifications (protocols, interfaces) separate stable developer-facing APIs from swappable internal implementations. Ensures framework survives 10+ years as tech evolves.

**5. Boring Technology Wins**
Prefer 5+ years proven, used by major companies, stable, low abstraction, web standards.

**6. Architectural Honesty Over Clever Workarounds**
Problems should be obviously painful during development, not hidden. Slow page? Fix queries or split page. Framework guides toward better patterns by making poor patterns difficult.

---

## HTTP-First Specific Principles

**7. Start Simple, Add Complexity Only When Proven Necessary**
Simple first, complex when needed.

**8. Validate Assumptions Before Building Infrastructure**
Proof before infrastructure—don't build WebSocket until component model works.

**9. Developer Experience is Not Negotiable**
Every design decision must answer: Does this make developers' lives better? DX is the #1 priority.

**10. Preserve Options as Long as Possible**
HTTP-first preserves options. WebSocket-first removes them. Don't foreclose options prematurely.

**11. Respect the 70%**
70% of web apps don't need WebSocket. HTTP-only should be first-class, not a degraded fallback. Serve the majority well, enable the minority.

**12. No Breaking Changes, Only Additions**
Backwards compatibility is sacred. v1.0 apps should work in v2.0 without modification. Respect your users' code.

**13. Ship Fast, Learn Fast, Iterate Fast**
Time to learning is the critical metric. HTTP-First gets us there faster.

**14. Build for the 70%, Enable the 30%**
Target audience clarity prevents feature creep.

---

## The Mantra

```
We are building:
- Turn-based architecture for web apps
- Server authority with thin client (6-8KB)
- Multi-language support (Node/Python/.NET/Go minimum)
- HTTP request/response (Phase 1: validate fundamentals)
- Server decides, client displays (fast round-trips feel responsive)

Phase 1 (HTTP-First):
- Validate server authority works with 100-200ms latency
- Validate HTML diffing is fast and bandwidth-efficient
- Validate component model is intuitive
- Validate data pre-fetching prevents N+1 queries
- Prove the architecture before complex transport

Phase 2 (WebSocket):
- Same component API, add Transport attribute
- Enable real-time features (server push, multi-user coordination)
- Smooth navigation (no page refreshes)
- Backwards compatible (HTTP components still work)

When in doubt, refer to game architecture:
- Does turn-based game need WebSocket? No (HTTP = taking turns)
- Does chess server run on client? No (server authority)
- Does game ship 500KB to client? No (thin client)
- Does Hearthstone work offline? No (server authority)

Neither should we.
```

---

## Conclusion

HTTP-First is a **validation strategy**, not a compromise. It's the belief that:
- Simple beats complex (HTTP before WebSocket)
- Validated beats assumed (prove fundamentals first)
- Incremental beats big bang (Phase 1 → Phase 2)
- Boring beats exciting (HTTP is proven for 26 years)
- Developer experience beats feature count (intuitive > complete)

It's the recognition that:
- 70% of web apps don't need WebSocket (CRUD, forms, admin)
- HTTP is first-class transport, not a fallback
- Constraints breed creativity (HTTP forces architectural clarity)
- Options are valuable (preserve ability to choose)
- Metrics beat opinions (data-driven go/no-go)

It's the commitment to:
- **Server authority with thin client** (game architecture principles)
- **Multi-language protocol-based design** (specification-first)
- **Start simple, validate early, iterate rapidly**
- **Respect developers** (their time, intelligence, needs)
- **Serve the majority, enable the minority** (70% HTTP, 30% WebSocket)

**HTTP-First is how we build a framework that developers will love—not because it has every feature, but because it respects their time, their intelligence, and their needs.**

---

## FAQ

**Q: Why not build WebSocket from the start?**
A: Validate core concepts first. WebSocket adds 3,200 LOC before we know if the foundation works.

**Q: Isn't HTTP-only a downgrade?**
A: No. HTTP is right for 70% of apps—simpler, more reliable, works everywhere.

**Q: What if we need real-time later?**
A: Phase 2. Same API, add `Transport = WebSocket` attribute. Quick migration.

**Q: How do you know 70% don't need WebSocket?**
A: Admin panels, CMS, e-commerce, forms, user management—none require real-time.

**Q: Is this just LiveView without WebSocket?**
A: No. Multi-language, HTTP-first (not fallback), set-based diffing (not vDOM), game-style data pre-fetching.

**Q: Why not just use Hotwire?**
A: Hotwire is Ruby-only. ArchType is multi-language with surgical diffs and declarative data pre-fetching.

**Q: When use WebSocket?**
A: Real-time (<1s latency), server push, multi-user coordination, presence. Chat, live dashboards, collaborative editing.

**Q: When stick with HTTP?**
A: CRUD, forms, admin panels, settings, user management, content management. If seconds of latency is acceptable.

**Q: Migration path?**
A: Add `[Component(Transport = TransportMode.WebSocket)]`, update client bundle, deploy. Simple and quick.

**Q: Don't believe in WebSocket?**
A: WebSocket is valuable for 30% that need real-time. Shouldn't be forced on 70% that don't.

---

**Version:** 1.0 • **Last Updated:** 2025-11-08 • **Next:** ARCHITECTURE.md
