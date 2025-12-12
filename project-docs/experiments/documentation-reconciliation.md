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
| 2 | Core Philosophy Docs | ✅ Complete |
| 3 | Pattern Template Docs | ✅ Complete |
| 4 | Read/Write Pattern Docs | ✅ Complete |
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

**Status:** ✅ Complete

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
- Created `README.PHILOSOPHY.md` (~750 lines) merging both philosophy docs
- Removed "NOTE - implementation has changed" disclaimers
- Removed hypothetical WebSocket transport code
- Removed ArchType references
- Updated all terminology (DataDomain → DomainView, DataManifest → DataDependencies, etc.)
- Updated code examples to use current types (TodosListDomainView)
- Updated `README.md` links to new 2-doc structure
- Deleted `README.WEB_FRAMEWORK.md`
- Deleted `README.CONSISTENT_COMPLEXITY_DESIGN.md`
- New structure: Philosophy (beliefs/why) + Architecture Reference (implementation/how)

---

## Phase 3: Pattern Template Docs

**Goal:** Update the pattern docs that AI/developers use when creating new features.

**Status:** ✅ Complete

### Target Files
- `project-docs/context/patterns/workflow-creation.md`
- `project-docs/context/patterns/test-creation.md`
- `.claude/docs/workflow/workflow-creation.md` (pointer only)
- `.claude/docs/workflow/test-creation.md` (pointer only)

### Research Tasks
- [x] Compare documented patterns to actual implementation
- [x] Identify what's missing (PageInteractionBase, DataMutations, etc.)
- [x] Check if test patterns still apply

### Known Issues (from initial scan)
- workflow-creation.md doesn't mention PageInteractionBase
- Doesn't show DataMutations pattern
- Doesn't show OOB response patterns
- May be missing DomainView provider patterns

### Research Findings

**File Structure Discovery:**
- `.claude/docs/workflow/workflow-creation.md` → Just a pointer to `project-docs/context/patterns/workflow-creation.md`
- `.claude/docs/workflow/test-creation.md` → Just a pointer to `project-docs/context/patterns/test-creation.md`
- Only 2 files with actual content to update

**workflow-creation.md Issues:**
- Broken reference to `README.CONSISTENT_COMPLEXITY_DESIGN.md` (deleted in Phase 2)
- Broken reference to `README.SYNTAX_STYLE.md` (doesn't exist)
- References auto-registration (`AutoRegisterWorkflows()`) which was removed
- Missing page feature patterns (View, Interaction, Provider)

**test-creation.md Issues:**
- Broken reference to `README.SYNTAX_STYLE.md` (doesn't exist)
- Test patterns for workflows still valid

### Plan Adjustments

**Decision: Enhance Architecture Doc Instead of Creation Docs**

Rather than expanding workflow-creation.md with all patterns, we added a comprehensive "Creating New Features" section to `README.ARCHITECTURE_REFERENCE.md` covering:
- Page feature creation (Provider → View → Interaction → Endpoints → DI)
- API workflow creation
- DI registration requirements
- Common mistakes table

Creation docs now reference the architecture doc for page features, keeping them focused on API workflows only.

### Completion Notes
- Added "Creating New Features" section to `README.ARCHITECTURE_REFERENCE.md` (~170 lines)
  - Step-by-step guide for page features (View + Interaction + Provider)
  - API workflow creation guide
  - Common mistakes table
- Added "Testing" section to `README.ARCHITECTURE_REFERENCE.md` (~65 lines)
  - SetupTestFor<T> pattern
  - Unit tests with Mocker
  - Integration tests with Fakes
- Added "Authentication" section to `README.ARCHITECTURE_REFERENCE.md` (~30 lines)
  - .RequireAuthenticatedUser() pattern
  - LoggedInAuthContext usage
- Deleted `project-docs/context/patterns/workflow-creation.md`
- Deleted `project-docs/context/patterns/test-creation.md`
- Updated pointer files in `.claude/docs/workflow/` to reference architecture doc

---

## Phase 4: Read/Write Pattern Docs

**Goal:** Update the read/write pattern documentation to reflect current implementation.

**Status:** ✅ Complete

### Target Files
- `.claude/docs/workflow/read-write-pattern.md`

### Research Tasks
- [x] Audit actual workflow implementations for current patterns
- [x] Audit actual DomainView provider implementations
- [x] Document the interaction layer (PageInteractionBase)
- [x] Document DataMutations and OOB flow

### Known Issues (from initial scan)
- Uses `IDataDomain` (should be `IDataProvider`)
- Uses string-based domain loading (should be fluent `With<T>().Load()`)
- Uses `GetDomain<T>("name")` (should be `Get<T>()`)
- Shows workflows returning query data (should return metadata only)
- Uses `DataDependencies.From<Provider, Context>()` (should be `From<Context>()`)
- Missing PageInteractionBase entirely
- Uses "CQRS" terminology throughout

### Research Findings
- Extensive terminology updates needed (IDataDomain → IDataProvider, etc.)
- All code examples used old API patterns
- Significant overlap with Architecture Reference doc
- CQRS framing could attract unwanted "corrections" from pattern purists

### Plan Adjustments
**Decision: Merge into Architecture Reference, delete standalone doc**

Rather than updating the standalone doc, consolidated the valuable content (decision tree, anti-patterns, guidance) into `README.ARCHITECTURE_REFERENCE.md` and deleted the original file. This:
- Reduces duplication
- Creates single source of truth
- Removes CQRS terminology in favor of "read vs write" framing

### Completion Notes
- Added "Read vs Write: Choosing the Right Pattern" section to `README.ARCHITECTURE_REFERENCE.md` (~120 lines)
  - Decision tree for Workflow vs Provider choice
  - "Why This Separation Matters" explanation (no CQRS jargon)
  - Three anti-pattern examples with correct alternatives
  - "When to Create New vs Extend Existing" guidance
- Deleted `.claude/docs/workflow/read-write-pattern.md`
- No pointer files existed for this doc

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
- Implemented Phase 2:
  - Created README.PHILOSOPHY.md with 10 sections (~750 lines)
  - Updated README.md links
  - Deleted old philosophy docs
  - All terminology updated to current implementation

### Session 4 (2025-12-12)
- Completed Phase 3: Pattern Template Docs
- Discovered `.claude/docs/workflow/*.md` files are just pointers to `project-docs/context/patterns/`
- Decision: Consolidate all creation guidance into architecture doc
- Added to README.ARCHITECTURE_REFERENCE.md:
  - "Creating New Features" section (~170 lines)
  - "Testing" section (~65 lines) - SetupTestFor<T>, Mocker, Fakes
  - "Authentication" section (~30 lines) - RequireAuthenticatedUser, LoggedInAuthContext
- Deleted creation docs (workflow-creation.md, test-creation.md)
- Updated pointer files to reference architecture doc
- Architecture doc is now the single source for all creation/testing/auth patterns

### Session 5 (2025-12-12)
- Completed Phase 4: Read/Write Pattern Docs
- Analyzed read-write-pattern.md - extensive terminology and API updates needed
- Decision: Merge into Architecture Reference instead of updating standalone doc
- Key change: Removed all "CQRS" terminology per user request (avoid pattern purists)
- Added "Read vs Write: Choosing the Right Pattern" section (~120 lines):
  - Decision tree for Workflow vs Provider
  - "Why This Separation Matters" (no jargon)
  - Three anti-pattern examples
  - "When to Create New vs Extend" guidance
- Deleted read-write-pattern.md

---

## Open Questions

1. Should we create a migration guide for anyone who learned the old patterns?
2. How much historical context should experiment docs preserve?
3. Should the "source of truth" doc replace or supplement existing philosophy docs?
4. Are there any docs that should just be deleted rather than updated?

---

**Last Updated:** 2025-12-12 (Phase 4 Complete)
