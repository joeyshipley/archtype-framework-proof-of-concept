# Brief: [Name]

**Filename**: `project-docs/briefs/P[phase]_[category]_[feature].md` (Phase P000-P999, flexible based on project)
**Created**: YYYY-MM-DD HH:MM
**Status**: active | exploration | planning | implementation | completed

---

## 📋 CURRENT MODE: [exploration/planning/implementation]

**Must read each session:**
- `.claude/docs/README*.md` - Project philosophy and architecture
- `.claude/docs/workflow/[current-mode].md` - Mode-specific workflow (exploration/planning/implementation)

---

## Intent

**For:** [Which persona? Check `project-docs/context/personas/README.md` for options. Examples: eager-explorer, aspiring-novelist, platform-owner]

**Problem:** [What problem are they experiencing? What's painful/slow/impossible today?]

**Outcome:** [What becomes possible/easier/better when this exists? How does their experience improve?]

**Why Now:** [Why is this important to solve now? What's the cost of not doing this?]

---

_The above captures user value and drives all decisions below._

---

## Constraints
**What must remain true no matter what?**

Always include:
- Mission alignment (serves project goals?)
- Architectural decisions (reference relevant documentation)
- Project core constraints (from `.claude/docs/README.md`)

Add specific constraints:
- Technical constraints for this brief
- What can't change
- What must be preserved

---

## Signal
**How will we know this is right?**

### User Signal
How will users know this works? What will they say/feel/do differently?
- [User feedback/behavior - e.g., "Developers report code feels readable immediately"]
- [User experience - e.g., "Can distinguish variables from strings at a glance"]
- [Measurable change - e.g., "Reduces time to understand unfamiliar code"]

### Technical Signal
What tests prove it works?
- [Test scenario 1]
- [Test scenario 2]
- [Performance/quality metric]

### Outcome Signal
What ships at the end?
- [Deliverable 1]
- [Deliverable 2]
- [Quality bar met]

---

## Open Questions
**What don't we know yet?**

This section drives mode selection:
- Architecture questions → EXPLORATION
- Implementation questions → PLANNING
- No questions → IMPLEMENTATION

Examples:
- "What does [feature] even mean here?"
- "Is this about UI or architecture?"
- "Should this be one thing or multiple?"
- "How do we validate this works?"
- "What's the simplest approach?"

If unsure, list what you don't know. That's the point.

---

## 🔍 EXPLORATION FINDINGS
**Leave empty until Exploration mode**

**⚠️ CRITICAL**: This section captures architectural DECISIONS only, not implementation plans. If you find yourself writing "Create X with functions..." or "Phase A: Build Y...", STOP - that belongs in Planning Output.

**Status**: [ ] Not started | [ ] In progress | [x] Complete

**Architectural Decisions Made**:
- [Decision 1: What architectural approach chosen]
- [Decision 2: What design pattern chosen]

**Rationale**:
[Why this approach? What constraints led here? What requirements influenced this?]

**Planning Input** (what to build):
- [Key piece 1]
- [Key piece 2]
- [Key piece 3]

❌ **NO in this section**: Tasks, phases, estimates, implementation steps, test plans, file names, function signatures
✅ **YES in this section**: Decisions, rationale, high-level what to build

**Before leaving Exploration**:
- [ ] Mark status complete
- [ ] Update CURRENT MODE to planning
- [ ] Commit: "Brief XX: Exploration complete"
- [ ] **RESTART SESSION** before planning

---

## 📐 PLANNING OUTPUT
**Leave empty until Planning mode**

**Status**: [ ] Not started | [ ] In progress | [x] Complete

### Current State Analysis
[What exists, what's missing]

### Dependencies
**CRITICAL: Map dependencies to prevent blocking**

- Which tasks depend on others?
- Which phases must complete before others start?
- What order minimizes blocking?

Example:
- Phase A before Phase B (B depends on A)
- Phase C parallel to B (independent)
- Phase D needs both B and C

### Task List

**🚨 CRITICAL: Follow the test cycle pattern (Write → Implement → Run All → Commit) for each test. Maximum 3 test cycles per phase.**

**📋 CHECKBOX TRACKING**: Task checkboxes `[ ]` are the **source of truth** for progress
- During implementation: Update `[ ]` → `[x]` at each **Commit** task
- On session restart: Find first unchecked `[ ]` task - that's where you resume
- Brief checkboxes persist across sessions (unlike TodoWrite which is ephemeral)
- See `.claude/docs/workflow/implementation-mode.md` for details

**Organize by phases with ATOMIC tasks:**

**Phase sizing rule**: Maximum 3 test cycles per phase. If more tests needed, create additional phases.

**Test cycle pattern**: Write Test → Implement → Run All Tests → Commit

#### Phase A: [Name]
1. [ ] Write Test: [Specific behavior A1]
2. [ ] Implement: [Make test pass]
3. [ ] **Run all tests**: Verify all green
4. [ ] **Commit**: "Test A1: [description]"
5. [ ] Write Test: [Specific behavior A2]
6. [ ] Implement: [Make test pass]
7. [ ] **Run all tests**: Verify all green
8. [ ] **Commit**: "Test A2: [description]"
9. [ ] Write Test: [Specific behavior A3]
10. [ ] Implement: [Make test pass]
11. [ ] **Run all tests**: Verify all green
12. [ ] **Commit**: "Test A3: [description]"
13. [ ] **STOP**: End session, resume at Phase B

#### Phase B: [Name]
14. [ ] Write Test: [Specific behavior B1]
15. [ ] Implement: [Make test pass]
16. [ ] **Run all tests**: Verify all green
17. [ ] **Commit**: "Test B1: [description]"
18. [ ] Write Test: [Specific behavior B2]
19. [ ] Implement: [Make test pass]
20. [ ] **Run all tests**: Verify all green
21. [ ] **Commit**: "Test B2: [description]"
22. [ ] **STOP**: End session, resume at Phase C

#### Phase X (Final): Integration Audit
N. [ ] **Update brief status**: Mark completed, move to project-docs/briefs/completed/
N+1. [ ] **Commit**: "Brief XX integration audit complete"
N+2. [ ] **STOP**: Brief complete, ready for next brief

**Each phase MUST**:
- Have maximum 3 test cycles (Write → Implement → Run → Commit)
- Each test cycle ends with "Run all tests" and "Commit"
- End with explicit "STOP" task (signals session boundary)
- Have tasks that are atomic and focused

**Example of CORRECT task breakdown**:
```markdown
#### Phase C: Error Formatting (2 test cycles)
11. [ ] Write Test: E001 error formatter structure
12. [ ] Implement: Basic error formatter
13. [ ] **Run all tests**: Verify all green
14. [ ] **Commit**: "Test: E001 error formatter structure"
15. [ ] Write Test: E001 shows constraint + reason + fix
16. [ ] Implement: Complete E001 error template
17. [ ] **Run all tests**: Verify all green
18. [ ] **Commit**: "Test: E001 complete error template"
19. [ ] **STOP**: End session, resume at Phase D
```

**Example of WRONG task breakdown** (batched commits):
```markdown
❌ Phase C: Error Formatting
11. [ ] Write Test: E001 error formatter structure
12. [ ] Implement: Basic error formatter
13. [ ] Write Test: E001 shows constraint + reason + fix
14. [ ] Implement: Complete E001 error template
15. [ ] **Run tests**: Verify all formatters pass
16. [ ] **Commit**: "Phase C: Error formatting complete"
   ⚠️ Should commit after EACH test cycle, not at end!
   ⚠️ Pattern should be: Write → Implement → Run → Commit (repeat)
```

### Multi-Session Strategy

**Recommended approach**: One phase per session to prevent context drift/slippage

**Single-Phase Sessions**:
- Session 1: Phase A only (natural commit point)
- Session 2: Phase B only (natural commit point)
- Session 3: Phase C only (natural commit point)
- Session 4: Phase D only (validation + completion)

**Why this works**:
- ✅ Clear "done" signal (phase complete + tests pass)
- ✅ Natural breakpoints every phase
- ✅ Lowest context drift risk
- ✅ Easy to resume (next session = next phase)
- ✅ Clean git history (one commit per phase)

**Track progress with TodoWrite + Brief checkboxes**:
- **TodoWrite**: All tasks loaded from Session 1
  - Tracks progress across sessions
  - Prevents forgetting tasks
  - User sees real-time progress
  - Mark in_progress/completed immediately
- **Brief checkboxes**: Persist across sessions
  - Source of truth for resume point
  - Find first unchecked task to resume

**Workflow**:
1. Planning session: Create plan, commit to brief
2. Implementation Session 1: TodoWrite all tasks, complete **Phase A only**, commit
3. Implementation Session 2: Continue from todos, complete **Phase B only**, commit
4. Implementation Session 3: Continue from todos, complete **Phase C only**, commit
5. Implementation Session 4: Complete **Phase D only**, validate, commit

### Implementation Approach
- **TDD**: Test first, implement to pass
- **Incremental**: Each phase builds on previous
- **Validation**: Test after each phase
- **Commits**: After each phase (git commit preserves state)
- **TodoWrite**: Mark in_progress before starting, completed immediately after

### Files Expected to Change
- [file/path] ([what changes])
- [test/path] ([what tests])

### Technical Decisions
- [Decision 1]: [Rationale]
- [Decision 2]: [Rationale]

### Risks/Unknowns
- [Risk 1]
- [Risk 2]

**Before leaving Planning**:
- [ ] Document approach
- [ ] Test validation after each phase
- [ ] Commit points after each phase
- [ ] Mark status complete
- [ ] Update CURRENT MODE to implementation

---

## ⚙️ IMPLEMENTATION PROGRESS
**Update during Implementation mode**

**Status**: [ ] Not started | [ ] In progress | [x] Complete

---

### 📍 Multi-Session Tracking

**Update at end of each session** so next session knows where to continue.

**TodoWrite Status**: [All tasks in TodoWrite from Session 1 start]
- Tasks 1-8: ✅ Completed (Session 1)
- Tasks 9-16: ⏳ In Progress (Session 2)
- Tasks 17-20: ⏸️ Pending (Session 3)

**Phase Progress**:
- [x] Phase A: [Name]
- [x] Phase B: [Name]
- [ ] Phase C: [Name] ← **NEXT**
- [ ] Phase D: [Name]

**Session Commits**:
- `abc123d` - Session 1: Phase A-B summary (YYYY-MM-DD)
- `def456e` - Session 2: Phase C-D summary (YYYY-MM-DD)

---

### Session Notes (Optional)

**Current Session**: YYYY-MM-DD Session N

**Files Modified**:
- [file] (changes)
- [test] (tests added)

**Notes**: [Session-specific observations, gotchas, or context]

---

### Completion Checklist

**Before completing brief** (handled by Phase X: Integration Audit):
- [ ] All implementation phases complete
- [ ] Tests passing
- [ ] Build succeeds
- [ ] Signal achieved
- [ ] Brief moved to project-docs/briefs/completed/

**Learnings**: [Key takeaways, patterns discovered, future considerations]

---

## Brief Lifecycle

**Active**: `project-docs/briefs/P[phase]_[category]_[feature].md`
- Work ongoing
- Enriched through modes
- Status shows current mode

**Completed**: `project-docs/briefs/completed/P[phase]_[category]_[feature].md`
- All modes complete
- Signal achieved
- Historical record

**Won't Do**: `project-docs/briefs/wont-do/P[phase]_[category]_[feature].md`
- Not pursuing
- Add note explaining why

---

## Example: Mid-Implementation Resume

This shows how to resume work across multiple sessions:

```markdown
## ⚙️ IMPLEMENTATION PROGRESS
Status: [~] In progress

### 📍 Multi-Session Tracking

**TodoWrite Status**: All 25 tasks in TodoWrite from Session 1
- Tasks 1-4: ✅ Completed (Session 1 - Phase A)
- Tasks 5-8: ✅ Completed (Session 2 - Phase B)
- Tasks 9-12: ⏸️ Pending ← **NEXT SESSION** (Session 3 - Phase C)
- Tasks 13-16: ⏸️ Pending (Session 4 - Phase D)
- Tasks 17-25: ⏸️ Pending (Session 5 - Phase X)

**Phase Progress**:
- [x] Phase A: Foundation
- [x] Phase B: Detection
- [ ] Phase C: Error Formatting ← **NEXT**
- [ ] Phase D: CLI Integration
- [ ] Phase X: Integration Audit

**Session Commits**:
- `abc123d` - Session 1: Phase A (2025-10-21)
- `def456e` - Session 2: Phase B (2025-10-21)
```

**What AI knows at Session 3 start:**
- TodoWrite has all 25 tasks, continue from Task 9
- Phase C is next (Tasks 9-12)
- Previous commits preserve context from Phases A-B
- Complete Phase C, commit, end session
- Phase X (Integration Audit) always comes last

---

## Usage Guide

### For Humans:
1. Use `.claude/docs/workflow/brief-creation.md` to create with AI assistance
2. Or manually create in `project-docs/briefs/P[phase]_[category]_[feature].md`
3. Fill Intent, Constraints, Signal, Open Questions
4. Leave mode sections empty (AI fills during work)
5. Start session, AI enters appropriate mode

### For AI:
1. Read brief at session start
2. Check CURRENT MODE and Status
3. **If implementation: check Phase Progress for next phase**
4. Read required files for mode
5. Do work for current mode
6. **At session end: update Multi-Session Tracking**
7. Update section before leaving mode
8. Update CURRENT MODE when transitioning
9. **When complete: Complete ALL items in Completion Checklist**
10. Move brief to project-docs/briefs/completed/
11. Only mark "Complete" after ALL checklist items done

---

**Created**: 2025-10-15
**Purpose**: Living document flowing through Exploration → Planning → Implementation
**Key Insight**: Brief is both work artifact and process guide
