# Consistent Complexity Design

**Version:** 1.0
**Last Updated:** 2025-11-22
**Status:** Foundation Document

---

## Core Principle

**Every vertical slice should have similar complexity.** When features vary wildly in implementation complexity, it indicates architectural problems—either the framework is missing abstractions, or the feature is too large and should be decomposed.

---

## What is Consistent Complexity?

In a well-designed system, adding a new feature should feel similar to adding the previous feature. The code structure, patterns, and amount of work should be predictable.

**Good Signs:**
- New CRUD feature takes ~2 hours, just like the last one
- Each workflow follows the same structure: validation → business logic → persistence
- Repository usage is identical across features
- Most files are 50-200 lines

**Bad Signs:**
- One feature is 50 lines, another is 500 lines
- Some features require custom infrastructure, others don't
- Copy-pasting code because abstraction doesn't fit
- "This feature is special" justifications

---

## Benefits

### For Junior Developers
When complexity is consistent, juniors can:
- Predict how long features will take
- Copy existing patterns with confidence
- Know when to ask for help (deviation from pattern)
- Build muscle memory through repetition

### For AI Agents
AI excels at pattern repetition:
- Consistent patterns → accurate code generation
- Predictable structure → fewer hallucinations
- Similar complexity → better estimates
- Clear conventions → less guidance needed

### For Teams
- **Faster code reviews** - reviewers know what to expect
- **Easier estimation** - story points become meaningful
- **Lower maintenance burden** - less cognitive switching between styles
- **Better knowledge sharing** - patterns transfer across features

---

## Vertical Slice Architecture

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
├── Projects/
│   ├── CreateProject/
│   ├── ListProjects/
│   └── UpdateProject/
└── Projects.Domain/
    ├── Models/
    └── Repository/
```

**Each feature is self-contained:**
- Request/Response contracts
- Validation logic
- Business workflow
- API endpoint

**Complexity is visible:**
- Feature has 10 files? Probably too complex, should decompose
- All features have 3-4 files? Good consistency
- One feature has custom infrastructure? Architecture smell

---

## Examples of Consistent Complexity

### CRUD Operations

Every CRUD workflow should look similar:

```csharp
// Create
public class CreateProjectWorkflow(IProjectRepository _repo)
{
    public async Task<CreateProjectResponse> ExecuteAsync(CreateProjectRequest request)
    {
        // 1. Validate
        // 2. Map to domain model
        // 3. Save via repository
        // 4. Return response
    }
}

// List
public class ListProjectsWorkflow(IProjectRepository _repo)
{
    public async Task<ListProjectsResponse> ExecuteAsync(ListProjectsRequest request)
    {
        // 1. Build specification from request
        // 2. Query via repository
        // 3. Map to response
        // 4. Return response
    }
}
```

**If one CRUD operation is 10x more complex**, ask:
- Is this feature actually CRUD, or something else?
- Are we missing framework support (pagination, filtering, sorting)?
- Should this be multiple smaller features?

---

## Self-Enforcing Patterns

Code should make the right thing easy and the wrong thing hard.

### Example: Migration Location

**Before (documented):**
```bash
# README: "Always use --output-dir for migrations"
dotnet ef migrations add MyMigration --output-dir Infrastructure/Database/Migrations
```
❌ Relies on memory, easy to forget

**After (enforced):**
```csharp
// AppDbContextFactory automatically uses correct location
// EF Core discovers existing migrations and follows their namespace
dotnet ef migrations add MyMigration
```
✅ Can't do it wrong

### Example: Repository Pattern

**Before (manual):**
```csharp
// Every feature manually writes SQL or EF queries
var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
```
❌ Inconsistent, easy to write N+1 queries

**After (abstracted):**
```csharp
// Generic repository + specifications
var user = await _userRepo.GetAsync(UserSpecifications.ByEmail(email));
```
✅ Consistent pattern, reusable specifications

---

## When Complexity Varies

Sometimes features genuinely differ in complexity. How do we handle this?

### Strategy 1: Extract Infrastructure

If many features need similar complexity, extract it to infrastructure:

```
Multiple features need pagination →
  Extract IPaginationService

Multiple features need file upload →
  Extract IFileStorageService

Multiple features need email →
  Extract IEmailService
```

Now all features using these services have consistent complexity again.

### Strategy 2: Decompose the Feature

If ONE feature is much more complex:

```
"User Management" feature is 500 lines →
  Break into: CreateUser, UpdateUser, DeleteUser, ListUsers, UserPermissions

Now each sub-feature is ~100 lines (consistent with others)
```

### Strategy 3: Accept the Complexity (Rarely)

Sometimes a feature is genuinely more complex:
- Payment processing with refunds, disputes, webhooks
- Real-time collaboration with CRDT merging
- Complex approval workflows with state machines

**When to accept:**
- Complexity is inherent to business domain (not accidental)
- Feature is well-isolated (doesn't leak complexity)
- Team explicitly documented why this is special

---

## Measuring Consistency

### Code Metrics
- **Lines per feature**: Should be within 2x range (50-100 or 100-200, not 50-500)
- **Files per feature**: Most features have 3-5 files
- **Dependencies per feature**: Similar import counts

### Time Metrics
- **Implementation time**: Similar story point estimates
- **Review time**: Most PRs take similar time to review
- **Onboarding time**: New dev can ship 2nd feature faster than 1st

### Smell Tests
- "This feature is different" explanations
- Custom abstractions for one feature
- Copy-paste with modifications (pattern doesn't fit)

---

## Patterns We Use

### DI Registration - Centralized
All dependency injection in `DependencyResolver.cs`:
```csharp
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IProjectRepository, ProjectRepository>();
```
✅ One place to see all registrations
❌ Never scatter `services.Add*` across Program.cs or elsewhere

### Repository Pattern - Generic + Specifications
```csharp
public interface IRepository<T>
{
    Task<T?> GetAsync(Specification<T> spec);
    Task<List<T>> ListAsync(Specification<T> spec);
    Task AddAsync(T entity);
}
```
✅ Every repository follows same contract
❌ Never write custom repository without good reason

### Migrations - Auto-discovered
```csharp
// AppDbContextFactory reads from appsettings.json
// EF Core finds migrations by namespace
```
✅ No flags, no manual paths
❌ Never hard-code connection strings

### Vertical Slices - Feature Folders
```
Feature/
  ├── Feature.Workflow.cs      # Business logic
  ├── Feature.BoundaryContracts.cs  # Request/Response
  └── Feature.Endpoint.cs       # API routing
```
✅ Everything related to feature in one place
❌ Never split by technical layer (Models/, Controllers/, Services/)

---

## For AI Agents

When implementing features, AI should:

1. **Look for existing patterns** - Find similar feature, copy structure
2. **Measure deviation** - If new feature is 2x bigger, ask why
3. **Reuse abstractions** - Use repositories, specifications, services
4. **Flag inconsistency** - "This feature seems more complex than others, should we decompose?"

AI should NOT:
- Create custom patterns for one feature
- Skip existing abstractions "because it's easier"
- Implement without checking similar features first

---

## Summary

**Consistent Complexity means:**
- Features feel similar in size and structure
- Patterns are reusable across features
- Junior devs and AI can predict what code looks like
- Deviations are signals to improve architecture

**We achieve this through:**
- Self-enforcing code patterns
- Vertical slice architecture
- Generic abstractions (repositories, specifications)
- Centralized infrastructure (DI, migrations, config)

**When complexity varies:**
- Extract to infrastructure (if many features need it)
- Decompose the feature (if one feature is too big)
- Accept rarely (and document why)

---

**The goal: A codebase where adding Feature N+1 feels like adding Feature N.**
