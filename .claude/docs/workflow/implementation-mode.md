---
- fail-fast
---

# IMPLEMENTATION MODE

**Purpose**: Know WHAT and HOW. Build it with TDD.

---

## What Is This Mode?

**Implementation is**:
- Test-first workflow
- Make tests pass
- Run full suite after changes
- System validation

**Assumes**:
- ✅ Architecture decided
- ✅ Plan approved
- ✅ TodoWrite task list exists
- ✅ Ready to code

**Not for**:
- Questioning architecture (exploration)
- Creating plans (planning)
- Skipping tests

---

## Start Checklist

### 1. Read Brief's Planning Output

**Check your project's brief location**
- /briefs

**Read Planning Output section**:
- Task list (find first unchecked `[ ]` task - that's where you are)
- Implementation approach
- Files to change

**Load tasks into TodoWrite**:
- Load unchecked tasks from current position (manageable batch for session)
- Brief checkboxes are source of truth (persist across sessions)
- TodoWrite is working memory (ephemeral, cleared on restart)

**🚨 VALIDATE TASK STRUCTURE**:

Check if tasks are organized in phases with proper structure:

```markdown
#### Phase A: [Name]
1. [ ] Write Test: [Specific behavior]
2. [ ] Implement: [Make test pass]
3. [ ] **Run tests**: Verify all green
4. [ ] **Commit**: "[Phase A summary]"
5. [ ] **STOP**: End session, resume at Phase B
```

**If tasks are NOT in phase structure with Run tests/Commit/STOP**:

❌ **STOP IMMEDIATELY**

**Present to user**:
> "⚠️ Planning incomplete: Tasks are not organized in atomic phases.
>
> Current task list is missing:
> - Phase organization (Phase A, Phase B, etc.)
> - Explicit 'Run tests' steps after each phase
> - Explicit 'Commit' steps after each phase
> - Explicit 'STOP' steps (session boundaries)
>
> This structure is required for:
> - Single-phase-per-session workflow
> - TodoWrite tracking across sessions
> - Clear completion criteria
> - TDD discipline enforcement
>
> **Recommendation**: Complete planning first before implementation.
>
> Should I:
> 1. Enter planning mode to create proper phase structure?
> 2. Continue anyway (not recommended - violates workflow)?
> 3. Something else?"

**Wait for user decision. Do NOT proceed with implementation.**

### 2. Load Architectural Context

**CRITICAL: Prevents drift to training data patterns**

Read relevant architectural documentation before implementing:
- `.claude/docs/README.md` - Project philosophy and team velocity principles
- `.claude/docs/README.CONSISTENT_COMPLEXITY_DESIGN.md` - Vertical slices and self-enforcing patterns
- `.claude/docs/README.SYNTAX_STYLE.md` - Code style conventions

**If task includes pattern references, read those:**
- `.claude/docs/patterns/workflow-creation.md` - When creating new workflows
- `.claude/docs/patterns/test-creation.md` - When creating test files

Pattern docs provide scaffolding templates. Execute in root session (no subprocess) to maintain full context.

### 3. Check TodoWrite

- What's in_progress?
- What's next?
- Where in implementation?

### 4. Verify Tests Pass

**Before starting**:
```bash
npm test
```

All tests must pass. Fix failures first.

### 5. Begin TDD

Follow workflow below.

---

## TDD Workflow (NO EXCEPTIONS)

### For Each Task

**1. Write Test First**
- Describe expected behavior
- Test should fail (red)
- Confirm failure message

**2. Implement Minimal Code**
- Simplest code to pass test
- No extra features
- Don't skip ahead

**3. Run Test**
- Should pass (green)
- Debug if fails

**4. Run Full Suite**

**After ANY core file modification**:

Identify your project's core files - typically:
- Entry points (CLI, main, index)
- Core logic/business rules
- Validators/parsers
- Critical infrastructure

```bash
npm test
```

**Fix failures immediately.**

**5. Mark Complete**

Update TodoWrite immediately.

When you reach a **Commit** task:
- Use Edit tool to update brief checkboxes `[ ]` → `[x]` for completed tasks
- Brief is source of truth across session restarts
- TodoWrite is working memory for current session

**6. Check for STOP Task**

**If next task says "STOP" or "End Phase"**:
1. ✅ Current phase complete
2. Run tests, commit changes
3. **STOP IMMEDIATELY - DO NOT PROCEED**
4. Present to user: "Phase X complete. [Summary]. Ready for Phase Y when you are."
5. **WAIT for user confirmation before continuing**

**NEVER proceed past a STOP task without explicit user confirmation.**

STOP tasks are mandatory session boundaries. They exist to:
- Prevent context collapse across phases
- Allow human review between phases
- Ensure atomic, reviewable progress
- Respect phase structure in briefs

**7. Next Task (if no STOP)**

Mark next in_progress, repeat.

---

## TDD Investigation (Bug Location Unknown)

**When unsure WHERE bug is**:

### Process

1. **Write test** for what SHOULD work
2. **Run test** - Pass or fail?
3. **Analyze**:
   - ✅ Passes → Bug in data flow (test data ≠ production data)
   - ❌ Fails → Bug in logic (implementation wrong)
4. **Find discrepancy** - Compare test vs production data
5. **Fix surgically** - Evidence-based change

### Why?

- Systematic hypothesis testing
- Prevents trial-and-error
- Isolates problem (logic/data/flow)
- Creates regression protection
- Documents scenario

### When?

- System-level errors (unclear location)
- Validation conflicts
- Expected vs actual comparison needed

---

## Critical Rules

### Verify Before Workarounds

**When something doesn't work, STOP**:

1. What's expected behavior per architectural decisions?
2. Bug in implementation or understanding?
3. Fix implementation or change approach?

**Don't**:
- Adapt code to fit buggy implementation
- Create workarounds without root cause investigation
- Assume compiler is correct

### Run Tests After Core Changes

Catches regressions immediately, not at session end.

### One Task In Progress

**Exactly ONE task in_progress**:
- Not zero (shows current work)
- Not multiple (prevents context switching)

### Complete Immediately

**Mark complete IMMEDIATELY after finishing**:
- No batching
- Real-time tracking
- Session crash protection

---

## Stop Conditions

**STOP implementation and reassess if**:

| Condition | What It Means | Action |
|-----------|---------------|--------|
| Any bugs during feature work | Implementation has issues | Stop, investigate root cause, plan fixes |
| Test passes, build fails | Data structure mismatch | Use TDD investigation process |
| Architecture feels wrong | Implementation reveals design flaw | Return to exploration |
| Plan not working | Approach needs redesign | Return to planning |
| Confusion compounds | Pushing through makes it worse | Stop, question, clarify |

**Don't push through confusion or systemic issues.**

---

## System Validation

**After implementation**:

### 1. Full Test Suite
```bash
npm test
```
All must pass.

### 2. Build/Run Application
```bash
# Use your project's build/run command
npm run build
# or
npm start
```
Must succeed.

### 3. Validate Output
- Output matches expectations
- Functionality works as intended
- Manual verification completed (if applicable)

### 4. Verify Goals
**If brief exists**: Check success criteria - achieved?
- All todos complete?
- Actually works as intended?
- Update brief status

---

## Session Completion Checklist

**ALL must be done**:

- [ ] All tests passing
- [ ] Code committed + pushed
- [ ] Brief updated (if exists)
- [ ] Brief moved to completed location (if exists)

**Only then: Session complete.**

---

## Anti-Patterns

| Pattern | Problem | Fix |
|---------|---------|-----|
| **Start without phase structure** | **No atomic tasks, no session boundaries** | **Validate task structure first, warn user, suggest planning** |
| **Proceed past STOP task** | **Violates phase boundaries, prevents review** | **Stop immediately, present status, wait for user** |
| Code before tests | No test coverage | Write test first, fail, implement |
| Skip full suite | Regressions undetected | Run after every core file change |
| Batch completions | Progress unclear | Mark complete immediately |
| Add extra features | Scope creep | Stick to task, other work = other tasks |
| Push through bugs | Context collapse | Use stop conditions |

---

## Return to Other Modes

| Signal | Mode | Why |
|--------|------|-----|
| Architecture fundamentally wrong | Exploration | Design flaw revealed |
| Plan not working | Planning | Approach needs redesign |
| New requirements discovered | Planning | Scope changed |

---

## Critical Constraints (Project-Specific)

**Define your project's non-negotiable rules here.**

Examples of constraint categories:
- **Technology choices**: Framework restrictions, language features, dependency policies
- **Code patterns**: Component design, error handling, naming conventions
- **Quality gates**: Testing requirements, build validation, type safety
- **Architecture rules**: Module boundaries, data flow, coupling limits

Document each constraint with:
- Tag/identifier for reference
- Clear rule statement
- Rationale or enforcement details

---

## Example Session

1. Read brief's Planning Output, load arch context (via tags)
2. Run tests (all pass)
3. Mark task in_progress
4. Write test (fails)
5. Implement (passes)
6. Run full suite (all pass)
7. Mark complete
8. Repeat
9. System validation
10. Update brief status
12. Move brief to completed location
13. Commit + push

---

**Remember**: Systematic, test-driven, incremental. Small steps, constant validation, clear tracking.

---

## 🚨 CHANGE PROTOCOL FOR THIS FILE 🚨

**This file defines the TDD implementation process. Changing it means changing how we write code.**

**If you think this file needs to change** (beyond constraint/example updates):

1. **STOP immediately**
2. Create change proposal document with date and description
3. Document:
   - What process you want to change
   - Why current process failed
   - What problem new process solves
   - Evidence from recent sessions (TDD violations, quality issues, etc.)
4. **DO NOT make the change**
5. Wait for human review in new session
6. Human decides if process should evolve or if drift is happening

**Updating constraints list is fine. Process changes require review.**
