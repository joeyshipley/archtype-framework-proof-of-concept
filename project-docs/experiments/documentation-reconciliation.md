# Documentation Reconciliation

**Status:** 🚧 In Progress
**Started:** 2025-12-12
**Goal:** Reconcile all documentation with actual implementation after architectural evolution

---

## Session Context Loading

**To resume this work, read these files:**
1. This file (for progress and context)
2. Current implementation reference:
   - `PagePlay.Site/Infrastructure/Web/Components/IView.cs` - Core view interface
   - `PagePlay.Site/Infrastructure/Web/Data/IDataProvider.cs` - Data provider interface
   - `PagePlay.Site/Infrastructure/Web/Pages/PageInteractionBase.cs` - Interaction base class
   - `PagePlay.Site/Infrastructure/UI/Elements/IElement.cs` - UI vocabulary base

---

## Problem Summary

Documentation was written at various points during architectural evolution. The code has since evolved through several experiments:
- Interface renames (IServerComponent → IView, IComponent → IElement)
- Data loading pattern changes (string-based → type-safe fluent API)
- CQRS enforcement (workflows return metadata only, not query data)
- Page/Component unification (pages ARE views now)
- OOB update automation (framework handles, not manual)

Many docs contain code examples and patterns that no longer match reality.

---

## High-Level Phases

| Phase | Focus | Status |
|-------|-------|--------|
| 1 | Create "Source of Truth" Reference | ✅ Complete |
| 2 | Core Philosophy Docs | 🔬 Research Complete |
| 3 | Pattern Template Docs | ⏳ Pending |
| 4 | Workflow/CQRS Docs | ⏳ Pending |
| 5 | UI/Styling Docs | ⏳ Pending |
| 6 | Historical Experiment Cleanup | ⏳ Pending |
| 7 | Final Review & Validation | ⏳ Pending |

---

## Phase 1: Create "Source of Truth" Reference

**Goal:** Document what actually exists in the codebase now, creating a reference to compare all other docs against.

**Status:** ✅ Complete

### Research Tasks
- [x] Audit current interfaces and their purposes
- [x] Document actual data loading flow
- [x] Document actual interaction/OOB flow
- [x] Document actual UI rendering flow
- [x] Create architecture diagram (text-based)

### Deliverable
- [x] Create `.claude/docs/README.ARCHITECTURE_REFERENCE.md` - Single source of truth

### Files Examined
```
Infrastructure/Web/Components/IView.cs          ✓
Infrastructure/Web/Components/ViewFactory.cs    ✓
Infrastructure/Web/Components/ViewContext.cs    ✓ (added)
Infrastructure/Web/Data/IDataProvider.cs        ✓
Infrastructure/Web/Data/DataLoader.cs           ✓ (actual location)
Infrastructure/Web/Data/DataLoaderBuilder.cs    ✓ (added)
Infrastructure/Web/Framework/FrameworkOrchestrator.cs  ✓
Infrastructure/Web/Pages/PageInteractionBase.cs ✓
Infrastructure/Web/Mutations/DataMutations.cs   ✓ (added)
Infrastructure/UI/IElement.cs                   ✓ (actual location)
Infrastructure/UI/Rendering/HtmlRenderer.cs     ✓

Example implementations:
Pages/Todos/Todos.Page.cs                       ✓
Pages/Todos/Interactions/CreateTodo.Interaction.cs  ✓
Application/Todos/Perspectives/List/TodoList.DomainView.cs  ✓
Application/Todos/Perspectives/List/TodosList.Provider.cs   ✓
```

### Research Findings

**Core Interfaces (Actual):**
| Interface | File | Purpose |
|-----------|------|---------|
| `IView` | `IView.cs` | Server-rendered view with data dependencies |
| `IDataProvider<T>` | `IDataProvider.cs` | Query-side data fetcher |
| `IElement` | `IElement.cs` | Semantic UI building block |
| `IHtmlRenderer` | `HtmlRenderer.cs` | Element → HTML converter |
| `PageInteractionBase` | `PageInteractionBase.cs` | Command handler base class |
| `IFrameworkOrchestrator` | `FrameworkOrchestrator.cs` | Coordinates data loading + rendering + OOB |
| `IDataLoader` | `DataLoader.cs` | Fluent data loading API |
| `IViewContextParser` | `ViewContext.cs` | Parses client-side view context |

**Data Loading Pattern (Actual):**
```csharp
// Fluent type-safe API
var data = await _dataLoader.With<TodosListDomainView>().Load();
var todos = data.Get<TodosListDomainView>();
```

**Dependency Declaration (Actual):**
```csharp
public DataDependencies Dependencies => DataDependencies.From<TodosListDomainView>();
```

**Mutation Declaration (Actual):**
```csharp
protected override DataMutations Mutates => DataMutations.For(TodosListDomainView.DomainName);
```

**File Organization Discovery:**
- `IElement.cs` is at `Infrastructure/UI/IElement.cs` (not `Infrastructure/UI/Elements/`)
- `IDataLoader` interface is defined in `DataLoader.cs` (not separate file)
- No `IHtmlRenderer.cs` - interface defined in `HtmlRenderer.cs`

### Plan Adjustments
- File locations in other phases should reference actual paths discovered here
- Phase 4 should include `DataMutations.cs` in review

### Completion Notes
- Created `README.ARCHITECTURE_REFERENCE.md` with complete interface documentation
- Included two flow diagrams (initial load + interaction)
- Documented file organization pattern
- Added terminology quick reference

---

## Phase 2: Core Philosophy Docs

**Goal:** Consolidate and update philosophy documents, align terminology with current implementation.

**Status:** 🔬 Research Complete, Ready to Implement

### Original Target Files
- `.claude/docs/README.md`
- `.claude/docs/README.WEB_FRAMEWORK.md`
- `.claude/docs/README.CONSISTENT_COMPLEXITY_DESIGN.md`

### Research Tasks
- [x] Read each file and identify specific outdated sections
- [x] List terminology that needs updating
- [x] Identify concepts that are still valid vs need rewriting
- [x] Check for code examples that need updating

### Research Findings

**README.md (28 lines)**
- Minimal changes needed - just update links to new structure

**README.WEB_FRAMEWORK.md (550 lines)**
- Has "NOTE - implementation has changed" disclaimer (remove)
- Line 129: `DataManifest` → `DataDependencies`
- Lines 139-162: `DataDomains` terminology throughout → `DomainViews + Providers`
- Line 338: `TodoListPageComponent : Component` → should use `IView`
- Lines 338-345: Hypothetical WebSocket transport code (remove - future feature)
- Line 536: References "ArchType" (remove - future project name)
- **Philosophy content is solid** - HTTP-first rationale, game patterns, server authority all still valid

**README.CONSISTENT_COMPLEXITY_DESIGN.md (599 lines)**
- Has "NOTE - implementation has changed" disclaimer (remove)
- Lines 279-296: `TodosDomainContext` → `TodosListDomainView`
- Lines 460-475: "DataDomains handle all reads" → "DomainViews + Providers"
- Line 464: `TodosDomain`, `TodoAnalyticsDomain` → proper DomainView names
- Lines 487-534: Example code uses wrong type names
- **Pattern guidance is solid** - vertical slices, CQRS, consistent complexity all still valid

### Plan Adjustments

**Decision: Option C - Merge into 2 Docs**

Instead of updating 3 separate files, consolidate into clearer structure:

1. **Create `README.PHILOSOPHY.md`** (~800-900 lines)
   - Merge WEB_FRAMEWORK + CONSISTENT_COMPLEXITY
   - Both answer "why do we do things this way?"
   - Clear separation from technical reference

2. **Keep `README.ARCHITECTURE_REFERENCE.md`** (~400 lines)
   - Technical reference for "what exists and how to use it"
   - Already created in Phase 1

3. **Update `README.md`**
   - Update links to new 2-doc structure

4. **Delete old files**
   - `README.WEB_FRAMEWORK.md`
   - `README.CONSISTENT_COMPLEXITY_DESIGN.md`

**Proposed Structure for README.PHILOSOPHY.md:**
```
1. Core Insight (web apps as turn-based games)
2. Core Principles (server authority, thin client, HTTP-first, etc.)
3. HTTP-First Rationale (why HTTP, validation strategy, game patterns)
4. Consistent Complexity (vertical slices, self-enforcing patterns)
5. What We Embrace / What We Reject
6. For AI Agents and Developers (practical guidance)
```

**Changes to Apply:**
- Remove "NOTE - implementation has changed" disclaimers
- Remove hypothetical WebSocket/future code examples
- Remove ArchType references
- Update all terminology per translation table
- Update code examples to current types

### Completion Notes
<!-- Document what was done -->

---

## Phase 3: Pattern Template Docs

**Goal:** Update the pattern docs that AI/developers use when creating new features.

**Status:** ⏳ Pending

### Target Files
- `project-docs/context/patterns/workflow-creation.md`
- `project-docs/context/patterns/test-creation.md`
- `.claude/docs/workflow/workflow-creation.md`
- `.claude/docs/workflow/test-creation.md`

### Research Tasks
- [ ] Compare documented patterns to actual implementation
- [ ] Identify what's missing (PageInteractionBase, DataMutations, etc.)
- [ ] Check if test patterns still apply

### Known Issues (from initial scan)
- workflow-creation.md doesn't mention PageInteractionBase
- Doesn't show DataMutations pattern
- Doesn't show OOB response patterns
- May be missing DomainView provider patterns

### Research Findings
<!-- Fill in during research phase -->

### Plan Adjustments
<!-- Update plan based on research -->

### Completion Notes
<!-- Document what was done -->

---

## Phase 4: Workflow/CQRS Docs

**Goal:** Update the read/write pattern documentation to reflect current CQRS implementation.

**Status:** ⏳ Pending

### Target Files
- `.claude/docs/workflow/read-write-pattern.md`

### Research Tasks
- [ ] Audit actual workflow implementations for current patterns
- [ ] Audit actual DomainView provider implementations
- [ ] Document the interaction layer (PageInteractionBase)
- [ ] Document DataMutations and OOB flow

### Known Issues (from initial scan)
- Uses `IDataDomain` (should be `IDataProvider`)
- Uses string-based domain loading (should be fluent `With<T>().Load()`)
- Uses `GetDomain<T>("name")` (should be `Get<T>()`)
- Shows workflows returning query data (should return metadata only)
- Uses `DataDependencies.From<Provider, Context>()` (should be `From<Context>()`)
- Missing PageInteractionBase entirely

### Research Findings
<!-- Fill in during research phase -->

### Plan Adjustments
<!-- Update plan based on research -->

### Completion Notes
<!-- Document what was done -->

---

## Phase 5: UI/Styling Docs

**Goal:** Update UI vocabulary and styling documentation.

**Status:** ⏳ Pending

### Target Files
- `.claude/docs/README.DESIGN_STYLING.md`
- `project-docs/experiments/completed/styles/B-*.md`

### Research Tasks
- [ ] Check if IElement vocabulary is accurately documented
- [ ] Check if styling token system docs match implementation
- [ ] Verify theme authoring docs are current

### Known Issues (from initial scan)
- Uses `IComponent` (should be `IElement`)
- May have incomplete token implementation notes
- Check if "implementation has changed" notes are still relevant

### Research Findings
<!-- Fill in during research phase -->

### Plan Adjustments
<!-- Update plan based on research -->

### Completion Notes
<!-- Document what was done -->

---

## Phase 6: Historical Experiment Cleanup

**Goal:** Organize experiment docs so historical context is preserved but doesn't confuse.

**Status:** ⏳ Pending

### Target Files
- `project-docs/experiments/completed/component-first-architecture.md`
- `project-docs/experiments/completed/page-component-unification.md`
- `project-docs/experiments/completed/domain-data-manifests.md`
- `project-docs/experiments/completed/fluent-domain-loading.md`
- `project-docs/experiments/completed/loader-workflow-separation.md`

### Research Tasks
- [ ] Determine which experiments led to current state
- [ ] Add "Historical Context" headers to completed experiments
- [ ] Note which patterns were superseded
- [ ] Consider consolidating or archiving

### Approach Options
1. Add clear "HISTORICAL" banners to each
2. Move to `experiments/archive/` folder
3. Create summary doc linking to current implementation

### Research Findings
<!-- Fill in during research phase -->

### Plan Adjustments
<!-- Update plan based on research -->

### Completion Notes
<!-- Document what was done -->

---

## Phase 7: Final Review & Validation

**Goal:** Ensure all docs are consistent and nothing was missed.

**Status:** ⏳ Pending

### Tasks
- [ ] Cross-reference all docs against Phase 1 source of truth
- [ ] Check for orphaned docs that reference old patterns
- [ ] Verify code examples compile/make sense
- [ ] Update any command docs (.claude/commands/)
- [ ] Final terminology grep for old names

### Validation Checklist
- [ ] No references to `IServerComponent` (except historical)
- [ ] No references to `ComponentId` (except historical)
- [ ] No references to `IComponent` for views (except historical)
- [ ] No references to `IDataDomain` (except historical)
- [ ] No string-based domain loading examples
- [ ] All workflow examples show metadata-only responses

### Research Findings
<!-- Fill in during research phase -->

### Completion Notes
<!-- Document what was done -->

---

## Appendix: Terminology Translation Table

| Old Term | New Term | Notes |
|----------|----------|-------|
| `IServerComponent` | `IView` | Server-rendered views |
| `ComponentId` | `ViewId` | View identifier |
| `IComponent` | `IElement` | UI vocabulary building blocks |
| `ComponentBase` | `ElementBase` | Base record for elements |
| `data-component` | `data-view` | DOM attribute for tracking |
| `IDataDomain` | `IDataProvider` | Data fetching interface |
| `DataDomain` | `DomainView` / Provider | Query-side data |
| `LoadDomainsAsync(string[])` | `With<T>().Load()` | Fluent data loading |
| `GetDomain<T>(name)` | `Get<T>()` | Type-safe context access |
| `From<Provider, Context>()` | `From<Context>()` | Single generic parameter |
| `/turn/*` | `/interaction/*` | Route prefix |

---

## Session Log

### Session 1 (2025-12-12)
- Created this tracking document
- Performed initial gap analysis across all docs
- Identified 7 phases of work
- Key finding: Code examples are wrong in patterns, interfaces, data loading, and CQRS patterns

### Session 2 (2025-12-12)
- Completed Phase 1: Source of Truth Reference
- Audited all core interfaces (IView, IDataProvider, IElement, etc.)
- Traced data loading flow (fluent API: With<T>().Load())
- Traced interaction/OOB flow (PageInteractionBase → FrameworkOrchestrator)
- Traced UI rendering flow (IElement → HtmlRenderer)
- Created `.claude/docs/README.ARCHITECTURE_REFERENCE.md`
- Corrected file paths (IElement.cs location, DataLoader.cs contains interface)
- Documented actual code patterns with examples from TodosPage

### Session 3 (2025-12-12)
- Completed Phase 2 research
- Read all three target files and compared to ARCHITECTURE_REFERENCE
- Identified specific outdated terminology and code examples
- Decision: Merge WEB_FRAMEWORK + CONSISTENT_COMPLEXITY into single PHILOSOPHY doc
- Rationale: Both answer "why", keeps technical reference separate
- Documented plan with proposed structure and changes to apply
- Ready to implement Phase 2

---

## Open Questions

1. Should we create a migration guide for anyone who learned the old patterns?
2. How much historical context should experiment docs preserve?
3. Should the "source of truth" doc replace or supplement existing philosophy docs?
4. Are there any docs that should just be deleted rather than updated?

---

**Last Updated:** 2025-12-12 (Phase 2 Research Complete)
