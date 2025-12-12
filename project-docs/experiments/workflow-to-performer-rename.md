# Experiment: Rename Workflow to Performer

**Status:** In Progress
**Created:** 2025-12-12
**Goal:** Rename "Workflow" terminology to "Performer" throughout the codebase

---

## Progress Log

| Phase | Status | Commit | Notes |
|-------|--------|--------|-------|
| Phase 1: Infrastructure Core | ✅ Complete | `1d0ecdc` | Core interfaces renamed |
| Phase 2: Application Layer | ✅ Complete | - | Performers + contracts renamed |
| Phase 3: Infrastructure References | ✅ Complete | - | PageInteractionBase, DependencyResolver, Interactions updated |
| Phase 4: Tests | ✅ Complete | - | Test files renamed, all 19 tests pass |
| Phase 5: Documentation | ✅ Complete | - | Core docs, historical notes, considerations updated |
| Phase 6: Verification | 🔄 Next | - | - |

---

## Context & Rationale

### The Problem

As the architecture evolved, we separated concerns:
- **Reads** → Providers + DomainViews (query side)
- **Writes** → Workflows (command side)

"Workflow" originally implied a broader concept (potentially including reads, multi-step orchestration). Now that it's purely write operations, the name is a semantic mismatch.

### Why "Performer"?

| Considered | Rejected Because |
|------------|------------------|
| Command/Handler | CQRS jargon (MediatR uses Handler) |
| Action | Already used for HTMX URLs (`.Action("/url")`) |
| Executor | Enterprise-y |

**Performer wins because:**
- Ties to existing `Perform()` method name
- Pairs with Provider: "Provider provides data, Performer performs work"
- Human-readable, no jargon
- Fresh - no existing baggage

```
Read side:  Provider  → provides data  → .Fetch()
Write side: Performer → performs work  → .Perform()
```

---

## Scope Analysis

### Infrastructure (Core Interfaces) - 4 files

| Current | New |
|---------|-----|
| `Infrastructure/Core/Application/IWorkflow.cs` | `IPerformer.cs` |
| `Infrastructure/Core/Application/IWorkflowRequest.cs` | `IPerformerRequest.cs` |
| `Infrastructure/Core/Application/IWorkflowResponse.cs` | `IPerformerResponse.cs` |
| `Infrastructure/Core/Application/WorkflowBase.cs` | `PerformerBase.cs` |

### Application Layer (Implementations) - 9 files

| Current | New |
|---------|-----|
| `Application/Accounts/Login/Login.Workflow.cs` | `Login.Performer.cs` |
| `Application/Accounts/Register/Register.Workflow.cs` | `Register.Performer.cs` |
| `Application/Accounts/ViewProfile/ViewProfile.Workflow.cs` | `ViewProfile.Performer.cs` |
| `Application/Todos/Workflows/CreateTodo/CreateTodo.Workflow.cs` | `CreateTodo.Performer.cs` |
| `Application/Todos/Workflows/DeleteTodo/DeleteTodo.Workflow.cs` | `DeleteTodo.Performer.cs` |
| `Application/Todos/Workflows/ToggleTodo/ToggleTodo.Workflow.cs` | `ToggleTodo.Performer.cs` |
| `Application/Todos/Workflows/UpdateTodo/UpdateTodo.Workflow.cs` | `UpdateTodo.Performer.cs` |
| `Application/StyleTest/GetRandomNumber/GetRandomNumber.Workflow.cs` | `GetRandomNumber.Performer.cs` |

### Boundary Contracts - 8 files

Each has `*WorkflowRequest` and `*WorkflowResponse` classes to rename:
- `LoginWorkflowRequest` → `LoginRequest`
- `LoginWorkflowResponse` → `LoginResponse`
- (and so on for all 8)

**Decision:** Drop "Workflow" entirely from request/response names (cleaner: `LoginRequest` not `LoginPerformerRequest`)

### Directory Rename - 1 directory

| Current | New |
|---------|-----|
| `Application/Todos/Workflows/` | `Application/Todos/Performers/` |

### Infrastructure References - 2 files

| File | Changes |
|------|---------|
| `Infrastructure/Web/Pages/PageInteractionBase.cs` | `IWorkflowRequest` → `IPerformerRequest`, `IWorkflowResponse` → `IPerformerResponse` |
| `Infrastructure/Dependencies/DependencyResolver.cs` | All `IWorkflow<>` → `IPerformer<>` registrations |

### Tests - 2 files

| Current | New |
|---------|-----|
| `PagePlay.Tests/.../LoginWorkflow.Unit.Tests.cs` | `LoginPerformer.Unit.Tests.cs` |
| `PagePlay.Tests/.../RegisterWorkflow.Unit.Tests.cs` | `RegisterPerformer.Unit.Tests.cs` |

### Documentation - 18 files

Files referencing "Workflow" that need updates:
- `.claude/docs/README.md`
- `.claude/docs/README.PHILOSOPHY.md`
- `.claude/docs/README.ARCHITECTURE_REFERENCE.md`
- `.claude/docs/workflow/brief-creation.md`
- `.claude/docs/workflow/phase-workflow.md`
- `.claude/docs/workflow/persona-creation.md`
- `project-docs/experiments/completed/*.md` (historical - add note only)
- `project-docs/plans/*.md`
- `project-docs/context/considerations/*.md`

---

## Implementation Phases

### Phase 1: Infrastructure Core (Foundation) ✅

Rename the core interfaces and base class. Everything else depends on this.

**Files:**
1. [x] `IWorkflow.cs` → `IPerformer.cs`
   - Interface: `IWorkflow<TRequest, TResponse>` → `IPerformer<TRequest, TResponse>`
2. [x] `IWorkflowRequest.cs` → `IPerformerRequest.cs`
   - Interface: `IWorkflowRequest` → `IPerformerRequest`
3. [x] `IWorkflowResponse.cs` → `IPerformerResponse.cs`
   - Interface: `IWorkflowResponse` → `IPerformerResponse`
4. [x] `WorkflowBase.cs` → `PerformerBase.cs`
   - Class: `WorkflowBase<TRequest, TResponse>` → `PerformerBase<TRequest, TResponse>`

**Verification:** [x] Project does NOT compile (expected - references broken)

---

### Phase 2: Application Layer (Implementations) ✅

Update all performer implementations and their boundary contracts.

**2a: Rename Workflow files to Performer files (8 files)** ✅

```
Login.Workflow.cs → Login.Performer.cs
Register.Workflow.cs → Register.Performer.cs
ViewProfile.Workflow.cs → ViewProfile.Performer.cs
CreateTodo.Workflow.cs → CreateTodo.Performer.cs
DeleteTodo.Workflow.cs → DeleteTodo.Performer.cs
ToggleTodo.Workflow.cs → ToggleTodo.Performer.cs
UpdateTodo.Workflow.cs → UpdateTodo.Performer.cs
GetRandomNumber.Workflow.cs → GetRandomNumber.Performer.cs
```

**2b: Update class names inside each file** ✅

```csharp
// Before
public class LoginWorkflow : WorkflowBase<LoginWorkflowRequest, LoginWorkflowResponse>,
    IWorkflow<LoginWorkflowRequest, LoginWorkflowResponse>

// After
public class LoginPerformer : PerformerBase<LoginRequest, LoginResponse>,
    IPerformer<LoginRequest, LoginResponse>
```

**2c: Update Boundary Contracts (8 files)** ✅

```csharp
// Before
public class LoginWorkflowRequest : IWorkflowRequest
public class LoginWorkflowResponse : IWorkflowResponse

// After
public class LoginRequest : IPerformerRequest
public class LoginResponse : IPerformerResponse
```

**2d: Rename directory** ✅

```
Application/Todos/Workflows/ → Application/Todos/Performers/
```

**Verification:** ✅ Project does NOT compile (25 errors - expected)

---

### Phase 3: Infrastructure References ✅

Update the infrastructure that consumes performers.

**3a: PageInteractionBase.cs** ✅

```csharp
// Before
where TRequest : IWorkflowRequest
where TResponse : IWorkflowResponse
IWorkflow<TRequest, TResponse> workflow

// After
where TRequest : IPerformerRequest
where TResponse : IPerformerResponse
IPerformer<TRequest, TResponse> performer
```

**3b: DependencyResolver.cs** ✅

```csharp
// Before
services.AddScoped<IWorkflow<LoginWorkflowRequest, LoginWorkflowResponse>, LoginWorkflow>();

// After
services.AddScoped<IPerformer<LoginRequest, LoginResponse>, LoginPerformer>();
```

**3c: Page Interactions** ✅

Updated all interaction files to use new Performer namespaces and types:
- `CreateTodo.Interaction.cs`
- `DeleteTodo.Interaction.cs`
- `ToggleTodo.Interaction.cs`
- `Authenticate.Interaction.cs`
- `GetRandomNumber.Interaction.cs`

**Verification:** ✅ PagePlay.Site compiles successfully

---

### Phase 4: Tests ✅

Update test file names and class references.

**Files:**
```
LoginWorkflow.Unit.Tests.cs → LoginPerformer.Unit.Tests.cs ✅
RegisterWorkflow.Unit.Tests.cs → RegisterPerformer.Unit.Tests.cs ✅
```

**Class names and references inside each test file.** ✅

**Verification:** ✅ All 19 tests pass

---

### Phase 5: Documentation ✅

Update all documentation to reflect new terminology.

**5a: Core docs (active, must update)** ✅
- `.claude/docs/README.md` - No changes needed (no Workflow references)
- `.claude/docs/README.PHILOSOPHY.md` - Updated all code examples and terminology
- `.claude/docs/README.ARCHITECTURE_REFERENCE.md` - Updated all code examples and terminology

**5b: Workflow docs (rename references, these are about process not the Workflow type)** ✅
- `.claude/docs/workflow/*.md` - No code pattern references found (these are about development process)

**5c: Historical docs (add note only)** ✅
- `project-docs/experiments/completed/loader-workflow-separation.md` - Added terminology note
- `project-docs/experiments/completed/component-first-architecture.md` - Added terminology note
- `project-docs/experiments/completed/domain-data-manifests.md` - Added terminology note

**5d: Other docs** ✅
- `project-docs/plans/phase-2-todos-component-conversion.md` - Added terminology note
- `project-docs/context/considerations/input-validation-sanitization.md` - Updated to PerformerRequest
- `project-docs/context/considerations/sql-injection-protection.md` - Updated to Performer Pattern
- `project-docs/context/considerations/file-upload-security.md` - Updated to Performer Pattern

---

### Phase 6: Verification

**6a: Build verification**
- [ ] `dotnet build` succeeds
- [ ] No warnings related to rename

**6b: Test verification**
- [ ] `dotnet test` all pass
- [ ] No skipped tests

**6c: Runtime verification**
- [ ] App starts
- [ ] Login works
- [ ] Register works
- [ ] Todos CRUD works
- [ ] StyleTest random number works

**6d: Code search verification**
- [ ] `grep -r "Workflow" --include="*.cs"` returns 0 matches (excluding comments)
- [ ] `grep -r "IWorkflow" --include="*.cs"` returns 0 matches

---

## Open Questions

### 1. Request/Response Naming

**Question:** Should we use `LoginPerformerRequest` or just `LoginRequest`?

**Options:**
- **Option A:** `LoginPerformerRequest` / `LoginPerformerResponse` - Explicit, matches pattern
- **Option B:** `LoginRequest` / `LoginResponse` - Cleaner, shorter

**Recommendation:** Option B - "Performer" is an implementation detail, request/response are the contract

**Decision:** Option B (cleaner)

---

### 2. Variable Naming in PageInteractionBase

**Question:** Rename `workflow` parameter to `performer`?

```csharp
// Current
IWorkflow<TRequest, TResponse> workflow

// Option A
IPerformer<TRequest, TResponse> performer

// Option B
IPerformer<TRequest, TResponse> handler  // generic term
```

**Recommendation:** Option A - Consistency with type name

**Decision:** Option A

---

## Success Criteria

- [ ] All "Workflow" references in `.cs` files replaced with "Performer"
- [ ] All tests pass
- [ ] App runs correctly
- [ ] Documentation updated
- [ ] No regression in functionality

---

## Rollback Plan

If issues arise:
1. Git revert to commit before Phase 1
2. All changes are file renames + find/replace, easily reversible

---

## Session Resume Protocol

1. Read this document
2. Check which phase was last completed
3. Continue from next unchecked phase
4. Run verification after each phase

---

**Ready to implement when approved.**
