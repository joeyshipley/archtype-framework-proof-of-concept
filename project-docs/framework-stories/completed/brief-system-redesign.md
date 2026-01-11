# Experiment: Brief System Redesign

**Status:** Complete
**Created:** 2025-12-12
**Goal:** Replace mode-based brief system with lightweight research-first approach

---

## Context Loading

**Read on resume:**
- This file
- `.claude/docs/briefs/PRODUCT_BRIEF_TEMPLATE.md` (current template)
- `.claude/docs/workflow/` files (current mode system)

---

## Problem

The current brief system has ~1,700 lines across 8 files with three sequential modes (Exploration → Planning → Implementation). This creates friction:

1. **Upfront planning trap** - TDD plans are written before research, leading to guesswork
2. **Mode ceremony** - Switching modes requires reading mode-specific files, updating CURRENT MODE, committing
3. **Rigid phase structure** - Max 3 test cycles per phase, explicit STOP markers
4. **Context duplication** - Multi-session tracking in 3 places (TodoWrite status, Phase Progress, Brief checkboxes)

The documentation-reconciliation experiment proved a lighter pattern works better: research discovers the path, plans emerge just-in-time.

---

## Success Signal

- Brief template under 100 lines
- Total workflow docs under 400 lines (down from ~1,700)
- No mode-switching ceremony
- Phase goals are "what" statements, TDD plans emerge from research
- Can resume work by reading one file (the brief itself)

---

## Constraints

- Must preserve TDD discipline for feature work (test-first remains non-negotiable)
- Must support multi-session work with clear handoff
- Must keep strong initial brief (Goal/Problem/Success Signal/Constraints as invariants)
- Phase goals must be stable; implementation details can flex

---

## Phases

| # | Goal | Status |
|---|------|--------|
| 1 | Create new brief template | ✅ |
| 2 | Create phase workflow doc | ✅ |
| 3 | Update brief creation workflow | ✅ |
| 4 | Update entry points and commands | ✅ |
| 5 | Cleanup old files | ✅ |

---

## Phase 1: Create New Brief Template

**Goal:** Lightweight template with invariants + phase structure

### Research
- [x] Read current PRODUCT_BRIEF_TEMPLATE.md
- [x] Identify what's essential vs ceremony
- [x] Design new structure based on documentation-reconciliation learnings

### Findings

**Essential (keep):**
- Goal, Problem, Success Signal, Constraints (invariants)
- Phase structure with goals
- Research → Findings → Done pattern per phase
- Session log for handoff
- Open questions section

**Ceremony (remove):**
- CURRENT MODE tracking
- Exploration Findings / Planning Output / Implementation Progress as separate sections
- Rigid test cycle patterns (Write → Implement → Run → Commit × 3)
- Multi-session tracking in 3 places
- 200 lines of examples embedded in template
- Intent section with For/Problem/Outcome/Why Now (redundant with Goal/Problem)

**New insight:** TDD plan should be a section within each phase, generated after research - not a separate mode output.

### Adjustments

Template structure:
```
Brief invariants (set at creation, rarely change)
├── Goal
├── Problem
├── Success Signal
└── Constraints

Phase table (overview)

Per-phase sections (filled progressively)
├── Research (checkboxes)
├── Findings (populated during research)
├── Adjustments (decisions based on findings)
├── TDD Plan (generated after research, if applicable)
└── Done (what shipped)

Open Questions
Session Log
```

### TDD Plan
> N/A - This phase creates a template file, not code

### Done
- Created `.claude/docs/briefs/BRIEF_TEMPLATE.md` (93 lines)
- Down from 418 lines in old template (78% reduction)
- Structure: Invariants → Phase table → Per-phase sections → Open Questions → Session Log
- Per-phase sections: Research → Findings → Adjustments → TDD Plan → Done

---

## Phase 2: Create Phase Workflow Doc

**Goal:** Single doc covering Research → TDD Plan → Execute cycle

### Research
- [x] Extract TDD essentials from planning-mode.md
- [x] Extract execution essentials from implementation-mode.md
- [x] Identify minimum viable process

### Findings

**Current state:** 3 mode files totaling ~948 lines
- exploration-mode.md: 188 lines
- planning-mode.md: 363 lines
- implementation-mode.md: 397 lines

**TDD Essentials (from planning-mode.md):**
- Context gathering (understand what exists, map dependencies)
- ATOMIC task breakdown with TDD pairs (Test → Implement alternating)
- Phase structure - each phase ends with: Run tests → Commit
- Present plan for approval before implementation
- Dependencies mapped explicitly

**Execution Essentials (from implementation-mode.md):**
- TDD Workflow: Test first (red) → Implement minimal (green) → Run test → Full suite
- Run tests after any core file change
- One task in_progress, mark complete immediately
- Stop conditions (bugs, architecture wrong, plan not working, confusion)
- System validation (full suite, build, verify goals)
- TDD Investigation for unknown bugs

**From exploration-mode.md:**
- Question, discover, decide (folds into Research)
- Listen to human uncertainty
- Capture findings

**Minimum Viable Process:**

Modes are ceremony. What matters is the cycle per phase:

```
Research → TDD Plan → Execute → Validate
    │          │          │         │
    ▼          ▼          ▼         ▼
 Understand  ATOMIC    Test→Code  Full suite
 context     tasks      cycle     + commit
```

### Adjustments

**New doc structure (~100 lines target):**

```markdown
# Phase Workflow

## Research
- Read relevant files
- Understand what exists
- Map dependencies
- Record findings in brief

## TDD Plan (after research)
- ATOMIC tasks with TDD pairs
- Each task: Test → Implement
- Phase ends with: Run tests → Commit
- Get approval if significant

## Execute
- Test first (red)
- Implement minimal (green)
- Run full suite after core changes
- One task in_progress, mark complete immediately
- STOP on: bugs, architecture wrong, confusion

## Validate
- Full test suite passes
- Build succeeds
- Goals achieved
- Commit + update brief
```

**What to remove:**
- Mode-specific language ("not for exploration")
- Mode transition ceremony
- Separate "Planning Output" sections
- 200+ lines of examples and anti-patterns
- Change protocols (move to meta doc if needed)

### TDD Plan
> N/A - This phase creates a workflow doc, not code

### Done
- Created `.claude/docs/workflow/phase-workflow.md` (111 lines)
- Consolidates 3 mode files (948 lines) into single workflow
- Covers: Research → Plan → Execute → Validate cycle
- Includes TDD Investigation pattern for unknown bugs

---

## Phase 3: Update Brief Creation Workflow

**Goal:** Streamlined workflow for creating initial brief with Claude

### Research
- [x] Review current brief-creation.md
- [x] Identify essential questions vs overhead
- [x] Design conversation flow

### Findings

**Current state:** brief-creation.md is 224 lines

**Essential (keep):**
- Questions to extract Goal, Problem, Success Signal, Constraints, Open Questions
- Phase breakdown guidance
- File naming convention
- "Next steps" guidance

**Overhead (remove):**
- Mode selection logic (~40 lines) - modes are gone
- Intent section with For/Problem/Outcome/Why Now - redundant with Goal/Problem
- "Is this architecture, implementation, or bug?" question - mode hint no longer needed
- 3 detailed examples (~65 lines) - keep 1 short example max
- Anti-patterns section (~15 lines) - mode-specific
- Change protocol section (~25 lines) - meta, not workflow
- Context Reference Map complexity

**New conversation flow:**
```
1. Understand Intent (4 questions → 4 invariants)
   "What are you building/changing?" → Goal
   "What's broken or missing today?" → Problem
   "How will you know it works?" → Success Signal
   "What can't change?" → Constraints

2. Capture Unknowns
   "What don't we know yet?" → Open Questions

3. Break into Phases
   Identify logical chunks of work with goals

4. Create Brief
   Use template, fill invariants + phases

5. Start or Save
   Begin Phase 1 research, or save for later
```

**Key insight:** No mode selection needed - each phase follows Research → Plan → Execute → Validate cycle from phase-workflow.md.

### Adjustments

Target ~60-70 lines (down from 224 - ~70% reduction)

### TDD Plan
> N/A - This phase creates a workflow doc, not code

### Done
- Created `.claude/docs/workflow/brief-creation-v2.md` (84 lines)
- Down from 224 lines in original (62% reduction)
- Removed: mode selection, Intent section redundancy, anti-patterns, change protocol, 2 of 3 examples
- Simplified to 5-step flow: Understand Intent → Capture Unknowns → Break into Phases → Create Brief → Next Steps
- Integrated with phase-workflow.md for execution

---

## Phase 4: Update Entry Points and Commands

**Goal:** Simplified session-start and commands

### Research
- [x] Review current session-start.md
- [x] Review command files
- [x] Design simplified flow

### Findings

**session-start.md (103 lines):**
- Step 1: Read core files - keep
- Step 2: Collaboration Discipline - mode table needs removal
- Step 3: Brief creation reference - update to v2
- Step 4: "Identify Your Mode" - entire section is mode-based (remove)

**Commands (10 files):**
| Command | Lines | Action |
|---------|-------|--------|
| /start | 1 | Keep, update target |
| /brief-create | 8 | Update references |
| /brief-check | 1 | Keep |
| /explore | 3 | Delete (mode-specific) |
| /explore-finalize | ? | Delete (mode-specific) |
| /plan | 3 | Delete (mode-specific) |
| /implement | 3 | Delete (mode-specific) |
| /persona | ? | Keep |
| /workflow-create | ? | Keep |
| /test-create | ? | Keep |

**Chain:** /start → AI_START.md → session-start.md (unnecessary indirection)

**Simplified flow:**
```
/start → Read core docs → "What are you working on?"
    ├── New work → Create brief → Start Phase 1 research
    └── Existing brief → Find current phase → Resume
```

No mode commands - each phase follows phase-workflow.md cycle.

### Adjustments

1. Create session-start-v2.md (~40-50 lines target)
2. Update /start to point directly to new session-start
3. Update /brief-create to reference new files
4. Mark mode commands for deletion in Phase 5

### TDD Plan
> N/A - This phase updates workflow docs, not code

### Done
- Created `.claude/docs/workflow/session-start-v2.md` (58 lines, down from 103 - 44% reduction)
- Removed mode selection logic, simplified to: Read docs → What are you working on?
- Updated `/start` command to point directly to session-start-v2.md (bypasses AI_START.md)
- Updated `/brief-create` command to reference new v2 files
- Mode commands (/explore, /plan, /implement) marked for deletion in Phase 5

---

## Phase 5: Cleanup Old Files

**Goal:** Remove old mode files, verify no broken references

### Research
- [x] List all files to delete
- [x] Search for references to deleted files
- [x] Verify nothing breaks

### Findings

**Files deleted (11 total):**

Workflow files (old):
- `brief-creation.md` (replaced by v2)
- `exploration-mode.md`
- `implementation-mode.md`
- `planning-mode.md`
- `session-start.md` (replaced by v2)

Brief templates (old):
- `PRODUCT_BRIEF_TEMPLATE.md` (replaced by BRIEF_TEMPLATE.md)

Commands (mode-specific):
- `explore.md`
- `explore-finalize.md`
- `implement.md`
- `plan.md`

Other:
- `AI_START.md` (unnecessary indirection)

**References found and handled:**
- Commands (`/explore`, `/plan`, `/implement`) → deleted (no longer needed)
- Internal references in old files → deleted with the files
- Historical references in experiment docs → kept as historical record

**Files renamed (cleaner names since clean break):**
- `brief-creation-v2.md` → `brief-creation.md`
- `session-start-v2.md` → `session-start.md`

### Done
- Deleted 11 old files
- Renamed 2 v2 files to clean names
- Updated 3 references to use clean names
- Verified no broken references in active files

---

## Open Questions

1. ~~Should we keep any backward compatibility with old briefs, or clean break?~~ **Decision:** Clean break, no backward compatibility needed.
2. ~~Where should the template live?~~ **Decision:** Keep in `.claude/docs/briefs/` - files live with the project, each project can have different ones.

---

## Session Log

### 2025-12-12 - Session 1
- Analyzed current brief system (~1,700 lines across 8 files)
- Compared to documentation-reconciliation flow that worked well
- Identified key insight: research-first with TDD plans emerging just-in-time
- Designed new approach: invariants + phase goals, per-phase research → TDD plan
- Created this experiment doc
- Phase 1 research complete, ready to create template

### 2025-12-12 - Session 2
- Completed Phases 1-4 (template, workflow doc, brief creation, entry points)
- Phase 5: Cleaned up old files
  - Deleted 11 files (5 workflow, 1 template, 4 commands, 1 indirection)
  - Renamed v2 files to clean names
  - Updated all references
- **Experiment complete**

**Final metrics:**
| Metric | Old | New | Reduction |
|--------|-----|-----|-----------|
| Brief template | 418 lines | 93 lines | 78% |
| Core workflow docs | ~948 lines | 253 lines | 73% |
| Total system | ~1,700 lines | 346 lines | 80% |
| Mode files | 3 | 0 | 100% |
| Commands | 10 | 6 | 40% |

**Success signals achieved:**
- ✅ Brief template under 100 lines (93)
- ✅ Total workflow docs under 400 lines (346)
- ✅ No mode-switching ceremony
- ✅ Phase goals are "what" statements, TDD plans emerge from research
- ✅ Can resume work by reading one file (the brief itself)
