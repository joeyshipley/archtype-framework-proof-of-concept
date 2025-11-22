# Brief: [Name]

**Filename**: `project-docs/briefs/P[phase]_[category]_[feature].md` (Phase P000-P999, flexible based on project)
**Created**: YYYY-MM-DD HH:MM
**Status**: active | exploration | planning | implementation | completed

---

## 📋 CURRENT MODE: [exploration/planning/implementation]

### Required Reading:
- [ ] docs/context/MISSION.md (always)
- [ ] docs/workflow/[current-mode].md (exploration-mode/planning-mode/implementation-mode)
- [ ] Other relevant documentation (check TAG_INDEX.json for tags)

### Documentation Tags:
[Add relevant tags from TAG_INDEX.json - e.g., #api, #database, #security]

---

## Intent

**For:** [Who is this for? Specific role/persona - e.g., "developers reading unfamiliar codebases", "API consumers", "system administrators"]

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
- Project core constraints (from docs/context/MISSION.md)

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

**⚠️ CRITICAL**: Do NOT design or specify implementation here. This section captures architectural DECISIONS, not implementation plans. If you find yourself writing "Create X with functions..." or "Phase A: Build Y...", STOP - that belongs in Planning Output.

**Use `/explore-finalize` command** when ready to capture findings (prevents drift into planning/implementation).

**Status**: [ ] Not started | [ ] In progress | [x] Complete

**Architectural Decisions Made**:
- [Decision 1: What architectural approach chosen]
- [Decision 2: What design pattern chosen]

**Documented In** (if design docs exist):
- [Reference to docs/context/ or docs/standards/ documents]

**Rationale**:
[Why this approach? What constraints led here? What requirements influenced this?]

**Planning Input** (what to build):
- [Key piece 1]
- [Key piece 2]
- [Key piece 3]

❌ **NO in this section**: Tasks, phases, estimates, implementation steps, test plans, file names, function signatures
✅ **YES in this section**: Decisions, rationale, high-level what to build

**Before leaving Exploration**:
- [ ] Run `/explore-finalize` to capture findings cleanly
- [ ] Document decisions in docs/context/ or docs/standards/ (if significant)
- [ ] Update TAG_INDEX.json if new docs added
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

**🚨 CRITICAL: ALWAYS include explicit test + commit + STOP steps in EVERY phase**

**📋 CHECKBOX TRACKING**: Task checkboxes `[ ]` are the **source of truth** for progress
- During implementation: Update `[ ]` → `[x]` at each **Commit** task
- On session restart: Find first unchecked `[ ]` task - that's where you resume
- Brief checkboxes persist across sessions (unlike TodoWrite which is ephemeral)
- See `docs/workflow/implementation-mode.md` for details

**Organize by phases with ATOMIC tasks:**

#### Phase A: [Name]
1. [ ] Write Test: [Specific behavior]
2. [ ] Implement: [Make test pass]
3. [ ] **Run tests**: Verify all green
4. [ ] **Commit**: "[Phase A summary]"
5. [ ] **STOP**: End session, resume at Phase B

#### Phase B: [Name]
6. [ ] Write Test: [Specific behavior]
7. [ ] Implement: [Make test pass]
8. [ ] **Run tests**: Verify all green
9. [ ] **Commit**: "[Phase B summary]"
10. [ ] **STOP**: End session, resume at Phase C

#### Phase X (Final): Integration Audit
N. [ ] **Architecture Docs**: docs/context/ and docs/standards/ updated if architectural decisions made
N+1. [ ] **TAG_INDEX.json**: Updated if new docs added or tags changed
N+2. [ ] **Update brief status**: Mark completed, move to project-docs/briefs/completed/
N+3. [ ] **Commit**: "Brief XX integration audit complete"
N+4. [ ] **STOP**: Brief complete, ready for next brief

**Each phase MUST**:
- End with explicit "Run tests" task
- End with explicit "Commit" task
- End with explicit "STOP" task (signals session boundary)
- Have tasks that are atomic and focused
- Separate "Write Test" from "Implement"

**Example of CORRECT task breakdown**:
```markdown
#### Phase C: Error Formatting
11. [ ] Write Test: E001 error formatter structure
12. [ ] Implement: Basic error formatter
13. [ ] Write Test: E001 shows constraint + reason + fix
14. [ ] Implement: Complete E001 error template
15. [ ] **Run tests**: Verify all formatters pass
16. [ ] **Commit**: "Phase C: Error formatting complete"
17. [ ] **STOP**: End session, resume at Phase D
```

**Example of WRONG task breakdown** (missing steps):
```markdown
❌ Phase C: Error Formatting
11. [ ] Write Test: E001 error formatter structure
12. [ ] Implement: Basic error formatter
13. [ ] Write Test: E001 shows constraint + reason + fix
14. [ ] Implement: Complete E001 error template
   ⚠️ Missing "Run tests" step!
   ⚠️ Missing "Commit" step!
   ⚠️ Missing "STOP" step!
```

### Multi-Session Strategy

**Recommended**: One phase per session to prevent context drift/slippage

**Single-Phase Cycle** (Recommended):
- Session 1: Phase A only (natural commit point)
- Session 2: Phase B only (natural commit point)
- Session 3: Phase C only (natural commit point)
- Session 4: Phase D only (validation + completion)

**Why Single-Phase**:
- ✅ Clear "done" signal (phase complete + tests pass)
- ✅ Natural breakpoints every phase
- ✅ Lowest context drift risk
- ✅ Easy to resume (next session = next phase)
- ✅ Clean git history (one commit per phase)

**Session Management** (Option 4 + Option 1):
- **Option 4 (TodoWrite)**: All tasks in TodoWrite from Session 1
  - Tracks progress across sessions
  - Prevents forgetting tasks
  - User sees real-time progress
  - Mark in_progress/completed immediately
- **Option 1 (Single-Phase Sessions)**: One phase per session
  - Each session ends with phase complete + git commit
  - Preserves context between sessions
  - Clear completion criteria per session
  - User can review commits per phase

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
📍 **MULTI-SESSION TRACKING** 📍

**TodoWrite Status**: [All tasks in TodoWrite from Session 1 start]
- Tasks 1-8: ✅ Completed (Session 1)
- Tasks 9-16: ⏳ In Progress (Session 2)
- Tasks 17-20: ⏸️ Pending (Session 3)

**Session Commits**:
- `abc123d` - Session 1: [Phase A-B summary] (YYYY-MM-DD)
- `def456e` - Session 2: [Phase C-D summary] (YYYY-MM-DD)
- `ghi789f` - Session 3: [Phase E summary] (YYYY-MM-DD)

---
📍 **PHASE PROGRESS TRACKER** 📍

**Current Phase**: [Phase name]
**Completed**:
- [x] Phase A: [Name]
- [x] Phase B: [Name]
- [ ] Phase C: [Name] ← **NEXT**
- [ ] Phase D: [Name]

**Update at end of each session** so next session knows where to continue.

---

**Before completing brief** (handled by Phase X):
- All implementation phases complete
- Phase X (Integration Audit) complete
- Brief moved to project-docs/briefs/completed/

---

### Session Info:
- **Session**: YYYY-MM-DD Session N
- **Notes**: [Session notes/artifacts if needed]

### Files Modified:
- [file] (changes)
- [test] (tests added)

### Validation:
- [ ] Tests passing
- [ ] Build succeeds
- [ ] Validation complete

### Signal Achievement:
- [ ] Yes - [How achieved]
- [ ] No - [What's missing]

### Completion Notes:
[Learnings, gotchas, future considerations]

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

## Examples

### Example: Starts in Exploration
```markdown
Intent:
For: API consumers integrating with our service
Problem: No clear way to authenticate - some endpoints open, some closed, no docs on auth flow
Outcome: Developers can securely authenticate and understand the security model immediately
Why Now: Blocking v1.0 launch - can't ship without auth strategy

Constraints: Mission alignment, existing architecture, security requirements
Signal:
- User: "Auth flow is clear and obvious"
- Technical: All endpoints protected, auth tests pass
- Outcome: Complete auth implementation shipped

Open Questions:
- What authentication approach fits this system?
- Session-based or token-based?
- Where does auth live architecturally?
→ Architecture questions = EXPLORATION
```

### Example: Starts in Planning
```markdown
Intent:
For: API consumers making high-volume requests
Problem: No rate limiting allows abuse, potential service degradation for all users
Outcome: Fair usage enforced, service remains stable under load, abusers blocked automatically
Why Now: Production incident last week - one client overwhelmed the API

Constraints: Work with existing auth, no new dependencies
Signal:
- User: "My legitimate requests succeed, I get clear feedback if rate limited"
- Technical: API returns 429 when limit exceeded, 200 for valid requests
- Outcome: Rate limiting live in production

Open Questions:
- Token bucket vs sliding window algorithm?
- Redis or in-memory storage?
- How to handle distributed systems?
→ Implementation questions = PLANNING
```

### Example: Starts in Implementation
```markdown
Intent:
For: Users with optional email fields in their profile
Problem: Database query crashes when email is null, breaking profile page
Outcome: Profile page loads correctly regardless of email presence
Why Now: Affecting 15% of users right now - production bug

Constraints: Don't break existing tests, TDD workflow
Signal:
- User: "Profile page loads without errors"
- Technical: Test "query handles null email" passes
- Outcome: Bug fixed in production

Open Questions: None (bug clear, fix known)
→ No questions = IMPLEMENTATION
```

### Example: Mid-Implementation Resume (Single-Phase Cycles)
```markdown
## ⚙️ IMPLEMENTATION PROGRESS
Status: [~] In progress

📍 **MULTI-SESSION TRACKING** 📍

**TodoWrite Status**: All 25 tasks in TodoWrite
- Tasks 1-4: ✅ Completed (Session 1 - Phase A)
- Tasks 5-8: ✅ Completed (Session 2 - Phase B)
- Tasks 9-12: ⏸️ Pending ← **NEXT SESSION** (Session 3 - Phase C)
- Tasks 13-16: ⏸️ Pending (Session 4 - Phase D)
- Tasks 17-25: ⏸️ Pending (Session 5 - Phase X: Integration Audit)

**Session Commits**:
- `abc123d` - Session 1: Phase A - Foundation (2025-10-21)
- `def456e` - Session 2: Phase B - Detection (2025-10-21)

📍 **PHASE PROGRESS TRACKER** 📍
**Current Phase**: Phase C: Error Formatting
**Completed**:
- [x] Phase A: Foundation
- [x] Phase B: Detection
- [ ] Phase C: Error Formatting ← **NEXT**
- [ ] Phase D: CLI Integration
- [ ] Phase X: Integration Audit

→ AI knows: Session 3 = Phase C only (Tasks 9-12)
→ TodoWrite already has all tasks, just continue from Task 9
→ Commits abc123d + def456e preserve context from Phases A-B
→ Clear breakpoint: Implement Phase C, commit, end session
→ Phase X (Integration Audit) is always the final phase
```

---

## Usage Guide

### For Humans:
1. Use docs/workflow/brief-creation.md to create with AI assistance
2. Or manually create in `project-docs/briefs/P[phase]_[category]_[feature].md`
3. Fill Intent, Constraints, Signal, Open Questions
4. Add architecture tags
5. Leave mode sections empty (AI fills)
6. Start session, AI enters appropriate mode

### For AI:
1. Read brief at session start
2. Check CURRENT MODE and Status
3. **If implementation: check PHASE PROGRESS TRACKER for next phase**
4. Read required files for mode
5. Load architecture files by tags
6. Do work for current mode
7. **At session end: update PHASE PROGRESS TRACKER**
8. Update section before leaving mode
9. Update CURRENT MODE when transitioning
10. **When complete: CHECK CHECKLIST in Implementation section**
11. **Complete ALL items** (move to completed/)
12. Only mark "Complete" after ALL checklist items done

---

**Created**: 2025-10-15
**Purpose**: Living document flowing through Exploration → Planning → Implementation
**Key Insight**: Brief is both work artifact and process guide
