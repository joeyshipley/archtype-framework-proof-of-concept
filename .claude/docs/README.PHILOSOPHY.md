# PagePlay Philosophy

**Version:** 2.0
**Last Updated:** 2025-12-12
**Status:** Foundation Document

---

## Overview

This document captures the core beliefs and patterns that guide PagePlay development. Two fundamental insights shape everything we build:

1. **Web applications are turn-based games** - The game industry solved server-authoritative, responsive client experiences 20 years ago. We apply those proven patterns to web development.

2. **Every feature should feel the same** - Consistent complexity across vertical slices means developers (and AI) can predict what code looks like, ship faster, and maintain architectural integrity.

For implementation details and current interfaces, see [Architecture Reference](./README.ARCHITECTURE_REFERENCE.md).

---

## Core Insight: Web Apps as Turn-Based Games

**Web applications ARE turn-based games.**

The game industry solved this problem 20 years ago: rich, responsive client experiences with complete server authority over unreliable networks. We're applying proven game architecture to web development.

**Every web interaction is a turn:**
- User takes action (click button → HTTP POST)
- Server processes (game logic, validation)
- Server returns result (HTML diff)
- User sees result (DOM update)
- User takes next action (next turn)

**This isn't a metaphor. This is the actual architecture.**

```
E-commerce checkout = Turn-based game
├─ Player action: "Add to cart" (HTTP POST)
├─ Server validates: "In stock? Allowed?"
├─ Server updates state: Cart += item
├─ Server sends result: HTML diff (cart badge)
└─ Player sees update: Cart badge shows 3
```

When in doubt, refer to game architecture:
- Does turn-based game need WebSocket? No (HTTP = taking turns)
- Does chess server run on client? No (server authority)
- Does game ship 500KB to client? No (thin client)

---

## Core Principles

### 1. Server Authority (Non-Negotiable)

Server owns all state, logic, validation. Client owns rendering, event capture, visual predictions. Never: client calculates final state.

```
CLIENT (6-8KB):
├─ Receives HTML diffs from server
├─ Applies CSS styling
├─ Captures events
└─ Sends events only (not state)

SERVER:
├─ Maintains all state
├─ Processes events
├─ Validates business logic
├─ Renders HTML
└─ Sends HTML diffs
```

### 2. Thin Client (8-10KB Target)

Initial load speed, simplicity, server authority. Client bundle stays tiny (no framework bloat). Minimal abstractions, tree-shakeable modules.

### 3. Consistent Complexity

Every vertical slice should have similar complexity. When features vary wildly in implementation complexity, it indicates architectural problems—either the framework is missing abstractions, or the feature is too large and should be decomposed.

**Good Signs:**
- New CRUD feature takes ~2 hours, just like the last one
- Each workflow follows the same structure
- Most files are 50-200 lines

**Bad Signs:**
- One feature is 50 lines, another is 500 lines
- Some features require custom infrastructure
- "This feature is special" justifications

### 4. Self-Enforcing Patterns

Code should make the right thing easy and the wrong thing hard. Patterns are enforced through code rather than relying on documentation. If you find yourself fighting established patterns, ask whether the pattern needs to evolve for everyone, not just your use case.

### 5. Specification-First Architecture

Define clear contract boundaries where specifications (protocols, interfaces) separate stable developer-facing APIs from swappable internal implementations. Ensures framework survives 10+ years as tech evolves.

### 6. Boring Technology Wins

Prefer 5+ years proven in production, used by major companies, low abstraction, web standards.

- **HTTP** (1999, 26 years): Billions of servers, every language has libraries
- **PostgreSQL** (1996, 29 years): Battle-tested at scale, mature tooling
- **Redis** (2009, 16 years): Known performance characteristics

Boring = fewer production surprises, more Stack Overflow answers, better tooling.

### 7. Architectural Honesty

Problems should be obviously painful during development, not hidden behind framework magic. Slow page? Fix the queries or split the page. Too much data? Paginate or reduce. Framework exposes problems early and clearly.

### 8. Developer Experience First

A fast, scalable, feature-complete framework that developers hate is a failed framework. We optimize for DX first.

**Great DX Means:**
- Familiar concepts (HTTP request/response, standard debugging tools)
- Fast feedback loops (change → see result < 5 seconds)
- Progressive disclosure (simple things are simple, complex things possible)
- Clear mental models

---

## HTTP-First Rationale

### Why HTTP-First?

> **The fastest way to validate an idea is to build the simplest version that tests the core hypothesis.**

HTTP-First is a **validation strategy**, not a compromise. Before investing in WebSocket infrastructure, we prove that:
1. Server authority provides good UX with 100-200ms latency
2. HTML diffing is fast enough (<5ms) and bandwidth-efficient
3. Data pre-fetching prevents N+1 queries

If the foundation fails, WebSocket was wasted effort. If it succeeds, adding WebSocket becomes straightforward.

### HTTP as First-Class Citizen

HTTP is the primary transport for 70% of web applications. WebSocket is the enhancement for the 30% that need real-time.

**HTTP's Strengths:**
- Universal compatibility (works through every proxy, firewall, CDN)
- Familiar mental model (every developer knows request/response)
- Stateless infrastructure (scale horizontally, no sticky sessions)
- Proven at scale (30+ years, massive tooling ecosystem)

### The 70% Rule

Most web applications don't need WebSocket. Admin panels, CMS, e-commerce, settings pages, forms, user management, invoicing, time tracking—none require real-time.

**The Complexity Tax:** Every app using a WebSocket framework pays for it (learning concepts, infrastructure, debugging) even if they never use real-time features.

**Our approach:** Default is HTTP (simple, works for 70%). Opt-in to WebSocket only when needed. Don't force 100% of users to pay for features only 30% need.

### Game Patterns Applied

#### Pattern 1: Server Authority

Client is rich in presentation, thin in authority. Server owns all truth. This is non-negotiable.

#### Pattern 2: Delta Compression (HTML Diffing)

```
Old HTML: <ul id="todos"><li>Todo 1</li></ul>
New HTML: <ul id="todos"><li>Todo 1</li><li>Todo 2</li></ul>

Diff: {"op": "append", "path": "#todos", "value": "<li>Todo 2</li>"}

Bandwidth: 80 bytes vs 2KB full HTML
```

Game inspiration: Source Engine's delta compression (send only what changed).

#### Pattern 3: Data Pre-Fetching (Game Level Loading)

```
Game level loading:
├─ Discover all entities in level
├─ Load ALL assets in parallel (loading screen)
├─ Instantiate entities with loaded assets
└─ Start gameplay (everything ready, no stuttering)

View data loading:
├─ Declare data requirements (DataDependencies)
├─ Execute queries in parallel (framework handles)
├─ Mount view with data ready
└─ Render (no N+1 queries, no waterfalls)
```

Result: Clean "loading → interactive" transition, predictable performance.

### The Simplicity Imperative

> "Simplicity is about subtracting the obvious and adding the meaningful." — John Maeda

HTTP-First removes thousands of lines: WebSocket connection management, heartbeat logic, reconnection with backoff, transport fallbacks, broadcasting infrastructure, session recovery.

We keep the meaningful parts: view lifecycle, HTML diffing, data pre-fetching, server authority.

### Risk Mitigation Through Incrementalism

**Three Key Risks:**
1. **Technical:** Do core concepts work? (server authority with latency, HTML diffing performance)
2. **Developer Experience:** Will developers adopt this? (intuitive model, helpful errors)
3. **Market:** Does this solve real problems? (simpler than alternatives)

**HTTP-First Reduces Risk:** Test core concepts in weeks (not months), get immediate developer feedback, validate before heavy investment.

---

## Consistent Complexity

### What is Consistent Complexity?

In a well-designed system, adding a new feature should feel similar to adding the previous feature. The code structure, patterns, and amount of work should be predictable.

### Benefits

**For Feature Development:** Developers focus on business logic, not infrastructure. Framework provides test bases, DI setup, repository patterns. Find similar feature, copy structure, ship faster.

**For AI Agents:** AI excels at pattern repetition. Consistent patterns → accurate code generation. Predictable structure → fewer hallucinations.

**For Architectural Integrity:** Constraints prevent individuals from breaking consistency with clever solutions. Code review focuses on "does this match the pattern?" not "is this clever?"

**For Teams:** Faster code reviews, easier estimation, lower maintenance burden, better knowledge sharing.

### Vertical Slice Architecture

We organize by feature, not by technical layer:

```
Application/
├── Accounts/
│   ├── Login/
│   │   ├── Login.Workflow.cs
│   │   ├── Login.BoundaryContracts.cs
│   │   └── Login.Endpoint.cs
│   ├── Register/
│   └── ViewProfile/
├── Accounts.Domain/
│   ├── Models/
│   └── Repository/
└── Projects/
    ├── CreateProject/
    ├── ListProjects/
    └── UpdateProject/
```

**Key principle: Start with single-method features**
- Each folder represents ONE operation (CreateProject, UpdateProject)
- No "UserManager" or "ProjectService" god classes
- Feature complexity is immediately visible by file count

### Self-Enforcing Patterns

**Example: Migration Location**

Before (documented):
```bash
# README: "Always use --output-dir for migrations"
dotnet ef migrations add MyMigration --output-dir Infrastructure/Database/Migrations
```
Relies on memory, easy to forget.

After (enforced):
```csharp
// AppDbContextFactory automatically uses correct location
dotnet ef migrations add MyMigration
```
Can't do it wrong.

### When Complexity Varies

**Strategy 1: Extract Infrastructure**
If many features need similar complexity, extract it:
- Multiple features need pagination → Extract IPaginationService
- Multiple features need file upload → Extract IFileStorageService

**Strategy 2: Decompose**
Don't build "User Management" as one feature. Start decomposed:
- CreateUser/ (one operation)
- UpdateUser/ (one operation)
- DeleteUser/ (one operation)

**Strategy 3: Accept (Rarely)**
Sometimes a feature is genuinely more complex (payment processing, approval workflows). Accept only when complexity is inherent to business domain, well-isolated, and explicitly documented.

---

## Patterns We Follow

### DI Registration - Centralized

All dependency injection in `DependencyResolver.cs`:
```csharp
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IProjectRepository, ProjectRepository>();
```
One place to see all registrations. Never scatter `services.Add*` across Program.cs.

### Repository Pattern - Generic + Specifications

```csharp
public interface IRepository<T>
{
    Task<T?> GetAsync(Specification<T> spec);
    Task<List<T>> ListAsync(Specification<T> spec);
    Task AddAsync(T entity);
}
```
Every repository follows same contract. Never write custom repository without good reason.

### Domain Loading - Fluent API

```csharp
// Single domain
var data = await dataLoader.With<TodosListDomainView>().Load();
var todos = data.Get<TodosListDomainView>();

// Multiple domains (same pattern!)
var data = await dataLoader
    .With<TodosListDomainView>()
    .With<AnalyticsDomainView>()
    .Load();
var todos = data.Get<TodosListDomainView>();
var analytics = data.Get<AnalyticsDomainView>();
```

Single consistent pattern for 1-N domains. No magic strings, compile-time safety. Never use `typeof()` - use generic type parameters.

### Vertical Slices - Single-Method Features

```
Feature/
  ├── Feature.Workflow.cs           # Business logic
  ├── Feature.BoundaryContracts.cs  # Request/Response
  └── Feature.Endpoint.cs           # API routing
```

Each folder = one operation. Everything related to feature in one place. Never create god classes. Never split by technical layer.

### Workflow Pattern - Revealing Intent

Every workflow follows consistent internal structure:

```csharp
public class RegisterWorkflow(
    IPasswordHasher _passwordHasher,
    IUserRepository _userRepository,
    IValidator<RegisterRequest> _validator
) : IWorkflow<RegisterRequest, RegisterResponse>
{
    // Entry point reveals business intent
    public async Task<IApplicationResult<RegisterResponse>> Perform(RegisterRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return response(validationResult);

        var user = createUser(request);

        var emailExists = await checkEmailExists(user.Email);
        if (emailExists)
            return response("An account with this email already exists.");

        await saveUser(user);
        return response();
    }

    // Response transformation - isolated
    private IApplicationResult<RegisterResponse> response(ValidationResult validationResult) =>
        ApplicationResult<RegisterResponse>.Fail(validationResult);

    private IApplicationResult<RegisterResponse> response(string errorMessage) =>
        ApplicationResult<RegisterResponse>.Fail(errorMessage);

    private IApplicationResult<RegisterResponse> response() =>
        ApplicationResult<RegisterResponse>.Succeed(
            new RegisterResponse { Message = "Account created successfully." }
        );

    // Implementation details - hidden
    private async Task<ValidationResult> validate(RegisterRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<bool> checkEmailExists(string email) =>
        await _userRepository.EmailExists(email);

    private User createUser(RegisterRequest request) =>
        User.Create(request.Email, _passwordHasher.HashPassword(request.Password));

    private async Task saveUser(User user)
    {
        await _userRepository.Add(user);
        await _userRepository.SaveChanges();
    }
}
```

**Benefits:**
- `Perform()` reads like a business process story
- Response shaping isolated from business logic
- Implementation details hidden in helper methods
- Type-safe ordering through data flow

### Read vs Write Separation

Clear separation between performing tasks and getting data to render:

**DomainViews + Providers handle all reads (queries):**
- Used by pages for initial data fetching
- Used by views for rendering
- Used by framework for OOB updates after mutations

**Workflows handle all writes (commands):**
- Create, Update, Delete operations
- Business logic, validation, authorization

**Pattern:**
- Need to read data? → Use DomainView via DataLoader
- Need to mutate data? → Create a Workflow
- No "read workflows" - reads go through DomainViews

```csharp
// Reading data through DomainView with fluent API
var data = await dataLoader.With<TodosListDomainView>().Load();
var todosData = data.Get<TodosListDomainView>();

// Mutating data through Workflow
public class CreateTodoWorkflow(ITodoRepository _repository)
    : IWorkflow<CreateTodoRequest, CreateTodoResponse>
{
    public async Task<IApplicationResult<CreateTodoResponse>> Perform(CreateTodoRequest request)
    {
        // validation, creation, persistence...
    }
}

// Don't create "ListTodos" workflow - use DomainView instead
```

### Testing Pattern - Framework-Provided Base

Every test inherits from `SetupTestFor<T>`:

```csharp
public class RegisterWorkflowUnitTests : SetupTestFor<RegisterWorkflow>
{
    [Fact]
    public async Task Perform_WithValidRequest_ReturnsSuccess()
    {
        Mocker
            .GetSubstituteFor<IUserRepository>()
            .EmailExists(Arg.Any<string>())
            .Returns(false);

        var result = await SUT.Perform(request);

        result.Success.Should().BeTrue();
    }
}
```

Developers never write test setup boilerplate. Adding dependencies doesn't break tests. Same base for unit and integration tests.

---

## What We Embrace

**1. Simplicity as a Feature**
The simplest solution that works is the best—easier to understand, debug, maintain, extend.

**2. Constraints as Gifts**
Constraints force better thinking. Removing complexity forced creative solutions and clarified what truly matters.

**3. Incremental Progress**
Small steps, fast feedback, course correction. Learn quickly, fail cheaply, iterate rapidly.

**4. Developer Empathy**
The developer is the user. Happy developers = better apps. Simple mental models = fewer bugs.

**5. Humility**
We don't know if everything will work. Let's find out quickly. Assumptions must be tested, metrics beat opinions.

---

## What We Reject

**1. Premature Optimization**
"We need X for maximum performance" → Reality: Simple is fast enough for most apps.

**2. Feature Completeness for v1**
"v1.0 must have every feature" → Reality: v1.0 must validate the core hypothesis.

**3. One Size Fits All**
"Every framework must support every use case" → Reality: It's OK to have a target audience.

**4. Complexity as Badge of Honor**
"More features = better framework" → Reality: Fewer features done well beats many done poorly.

**5. Technology for Technology's Sake**
"Newer is better" → Reality: Choose technology that fits the problem.

**6. "Special" Features**
"This feature is different" → Reality: If a feature needs special handling, extract to framework or decompose it.

---

## What We're NOT Building

- **NOT offline-first** - No Service Workers, no sync engines, no CRDTs
- **NOT progressive enhancement** - JavaScript required for all interactivity
- **NOT real-time continuous** - No 60+ updates per second, no client-side physics
- **NOT SPA** - No client-side routing/state/virtual DOM
- **NOT everything framework** - No built-in auth providers, error reporting, analytics

---

## For AI Agents and Developers

When implementing features:

1. **Use framework infrastructure** - Tests inherit from `SetupTestFor<T>`, workflows use repositories, DI is centralized
2. **Start with single operation** - Each feature folder does ONE thing
3. **Follow read/write separation** - DomainViews for queries, Workflows for commands
4. **Look for existing patterns** - Find similar feature, copy structure exactly
5. **Measure deviation** - If new feature is 2x bigger, ask why
6. **Flag complexity** - "This feature seems more complex than others, should we extract to framework?"

**When implementing reads:**
- Use DomainView via DataLoader with fluent API: `dataLoader.With<TView>().Load()`
- Access data with `data.Get<TView>()` (no magic strings)
- Never create "List" or "Get" workflows

**When implementing writes:**
- Create a Workflow for the mutation
- Workflow names reveal business actions
- Follow revealing intent pattern

**When infrastructure is missing:**
- Feature code reveals need for framework support
- Extract to framework infrastructure, not feature-level abstraction

**AI should NOT:**
- Create custom patterns for one feature
- Skip existing abstractions "because it's easier"
- Manually mock dependencies in tests
- Create god classes (UserManager, ProjectService)
- Put multiple operations in one feature folder

---

## FAQ

**Q: Why HTTP instead of WebSocket?**
A: HTTP is right for 70% of apps—simpler, more reliable, works everywhere. We validate core concepts first.

**Q: When should I use WebSocket?**
A: Real-time requirements (<1s latency), server push, multi-user coordination, presence. Chat, live dashboards, collaborative editing.

**Q: What if my feature seems more complex than others?**
A: Ask why. Either decompose it (too big), extract infrastructure (missing framework support), or document why it's genuinely special.

**Q: Can I deviate from the patterns?**
A: Ask whether the pattern needs to evolve for everyone, or if there's a way to solve your problem within the existing structure.

**Q: Why no client-side state?**
A: Server authority. Client displays what server says. This eliminates sync bugs, simplifies debugging, and matches game architecture.

**Q: How do I know if I'm doing it right?**
A: Your feature folder should look like other feature folders. If it doesn't, something is off.

---

## Summary

**Consistent Complexity means:**
- Features feel similar in size and structure
- Patterns are reusable across features
- Developers and AI can predict what code looks like
- Deviations signal opportunities to improve architecture

**We achieve this through:**
- Self-enforcing code patterns
- Vertical slice architecture
- Generic abstractions (repositories, specifications)
- Centralized infrastructure (DI, config)
- Server authority with thin client

**The goal: A codebase where adding Feature N+1 feels like adding Feature N.**

---

**Version:** 2.0 | **Last Updated:** 2025-12-12
