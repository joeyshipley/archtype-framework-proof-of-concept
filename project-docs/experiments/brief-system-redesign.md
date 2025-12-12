# Experiment: Brief System Redesign

**Status:** In Progress
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
| 1 | Create new brief template | ⬜ |
| 2 | Create phase workflow doc | ⬜ |
| 3 | Update brief creation workflow | ⬜ |
| 4 | Update entry points and commands | ⬜ |
| 5 | Cleanup old files | ⬜ |

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
[To be filled after implementation]

---

## Phase 2: Create Phase Workflow Doc

**Goal:** Single doc covering Research → TDD Plan → Execute cycle

### Research
- [ ] Extract TDD essentials from planning-mode.md
- [ ] Extract execution essentials from implementation-mode.md
- [ ] Identify minimum viable process

### Findings
[To be populated]

### Adjustments
[To be populated]

### Done
[To be filled]

---

## Phase 3: Update Brief Creation Workflow

**Goal:** Streamlined workflow for creating initial brief with Claude

### Research
- [ ] Review current brief-creation.md
- [ ] Identify essential questions vs overhead
- [ ] Design conversation flow

### Findings
[To be populated]

### Done
[To be filled]

---

## Phase 4: Update Entry Points and Commands

**Goal:** Simplified session-start and commands

### Research
- [ ] Review current session-start.md
- [ ] Review command files
- [ ] Design simplified flow

### Findings
[To be populated]

### Done
[To be filled]

---

## Phase 5: Cleanup Old Files

**Goal:** Remove old mode files, verify no broken references

### Research
- [ ] List all files to delete
- [ ] Search for references to deleted files
- [ ] Verify nothing breaks

### Findings
[To be populated]

### Done
[To be filled]

---

## Open Questions

1. Should we keep any backward compatibility with old briefs, or clean break?
2. Where should the template live? `.claude/docs/briefs/` or move to `project-docs/templates/`?

---

## Session Log

### 2025-12-12 - Session 1
- Analyzed current brief system (~1,700 lines across 8 files)
- Compared to documentation-reconciliation flow that worked well
- Identified key insight: research-first with TDD plans emerging just-in-time
- Designed new approach: invariants + phase goals, per-phase research → TDD plan
- Created this experiment doc
- Phase 1 research complete, ready to create template
