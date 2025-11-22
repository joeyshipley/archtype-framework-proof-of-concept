---
- incremental-development
---

# PLANNING MODE

**Purpose**: Know WHAT to build. Design HOW to build it.

---

## What Is This Mode?

**Planning creates**:
- ATOMIC task breakdown
- TDD approach
- Success criteria
- Implementation roadmap

**Assumes**:
- ✅ Architecture decided (exploration complete)
- ✅ Clear goal/spec
- ✅ Ready to design approach

**Not for**:
- Architectural discussions (exploration)
- Writing code (implementation)
- Questioning whether to build (exploration)

---

## Start Checklist

### 1. Read Brief

**Check `project-docs/briefs/` directory for `P[phase]_[category]_[feature].md`**

**Read**:
- Intent (the why)
- Constraints (what can't change)
- Signal (success criteria)
- Exploration Findings → **Planning Input** (what to build)

### 2. Load Context

Navigate to relevant docs via `project-docs/context/`

Ensures plan aligns with architectural decisions.

### 3. Present to Human

**Template**:
> "Goal: [state it]. Creating ATOMIC task plan. Starting with context gathering."

---

## Planning Workflow

**Note**: This section describes the 4-step **planning process** (how to create a plan). Your actual **implementation plan** should have as many phases as needed based on units of change - small and tight.

### Step 1: Context Gathering

**Goal**: Understand what exists

- Identify files to read
- Understand current implementation
- **Map dependencies** (CRITICAL - see Dependencies section below)
- Identify test coverage gaps

**Don't write code yet.**

### Step 2: Create Plan

**Structure**:
```markdown
## Goal
[What we're building and why]

## Success Criteria
[How we know it works]

## Dependencies
[What must complete before what - prevents blocking]

## Tasks (ATOMIC - organized by phases)

#### Phase A: [Name] - [Small unit of change]
1. [ ] Write Test: [specific behavior A1]
2. [ ] Implement: [make A1 test pass]
3. [ ] Write Test: [specific behavior A2]
4. [ ] Implement: [make A2 test pass]
5. [ ] **Run tests**: Verify all green
6. [ ] **Commit**: "[Phase A summary]"
7. [ ] **STOP**: End session, resume at Phase B

#### Phase B: [Name] - [Another small unit of change]
8. [ ] Write Test: [specific behavior B1]
9. [ ] Implement: [make B1 test pass]
10. [ ] **Run tests**: Verify all green
11. [ ] **Commit**: "[Phase B summary]"
12. [ ] **STOP**: End session, resume at Phase C

[Continue with as many phases as needed - each phase = one small, tight unit of change]
...
```

**Phase Guidelines**:
- Each phase = ONE small, coherent unit of change
- It's better to have MORE phases (10-15 small ones) than fewer large ones (3-4)
- Each phase should be independently committable
- Don't artificially limit yourself to a fixed number of phases

**🚨 CRITICAL TDD Pattern**:
- **Within each phase**: Alternate Test → Implement → Test → Implement (TDD pairs)
- **NOT**: All tests first, then all implementations
- **Run tests** after completing all TDD pairs in phase
- Each test-implement pair should be small and focused

**🚨 CRITICAL: Every phase MUST end with**:
- **Run tests** task (verify all green after all TDD pairs)
- **Commit** task (preserve state)
- **STOP** task (explicit session boundary)

### Step 3: Present and Iterate

**Present**:
- Plan summary
- Task count
- Risks/unknowns

**Ask**: "Does this plan look good, or should I make changes?"

**Wait for EXPLICIT approval**:
- ✅ "Yes, proceed" / "Looks good, commit it" / "Approve"
- ✅ "Make these changes: ..." (iterate, then re-present)
- ❌ Discussion about plan scope/size is NOT approval
- ❌ "I like it" without "proceed/commit" is NOT approval

**If unclear**: Ask "Should I commit this plan, or make changes first?"

**Iterate until explicitly approved.**

### Step 4: Commit and Prompt

**ONLY after explicit approval** ("Yes, commit" / "Proceed" / "Looks good, commit it"):

1. **Update brief's "Planning Output"**:
   - Copy full task list (all tasks with checkboxes)
   - Include dependencies section
   - Include implementation approach
   - Mark planning status complete
   - Keep CURRENT MODE as planning

2. **Commit and push**:
   - Add brief with planning output
   - Commit message: "Planning: Brief XX - [title]"
   - Push to remote

3. **Ask what's next**:
   - "Plan committed and pushed. What would you like to do next?"
   - "Options: 1) Start implementation 2) End session 3) Something else"

**CRITICAL**: DO NOT auto-start implementation. DO NOT use TodoWrite. WAIT for human to choose next action.

**Planning typically ends here** (human saves context for fresh implementation session).

---

## Critical: Dependency Mapping

**Key principle**: Tasks frequently have dependency issues when dependencies aren't explicitly mapped up front.

### What to Map

- Which tasks depend on others?
- Which phases must complete before others start?
- What order minimizes blocking?
- What can run in parallel?

### How to Map

**In Planning Output, create Dependencies section**:

```markdown
## Dependencies

### Phase A (Foundation)
- Task 1 → Task 2 (Task 2 needs Task 1 output)
- Task 3 (parallel to 1-2, independent)

### Phase B (Builds on A)
- Requires: Phase A complete
- Task 4 → Task 5 → Task 6

### Phase C (Validation)
- Requires: Phase B complete
- All validation tasks (can run parallel)
```

**Wrong ordering causes**:
- Implementation blocked mid-work
- Context switching between unrelated work
- Rework when dependencies discovered late

---

## Best Practices

### Test-First Always (TDD Cycle)

**Every implement task must be preceded by a test task.**

**The TDD rhythm**: Test → Implement → Test → Implement → Test → Implement

**NOT**: Test, Test, Test → Implement, Implement, Implement

| Good (TDD) | Bad (No TDD) |
|------|-----|
| 1. Write Test: Button renders<br>2. Implement: Button component<br>3. Write Test: Button handles clicks<br>4. Implement: Click handler | 1. Write Test: Button renders<br>2. Write Test: Button handles clicks<br>3. Write Test: Button has variants<br>4. Implement: Button component |

**Key principle**: Each test-implement pair should be small and focused. Run tests after implementation to verify green before moving to next test.

### Keep ATOMIC

| Too Big | ATOMIC (TDD pairs) |
|---------|--------|
| Implement JavaScript compiler | 1. Write Test: Parse function declaration<br>2. Implement: Function declaration parsing<br>3. **Run tests** (verify green)<br>4. Write Test: Compile function to method<br>5. Implement: Function to method compilation<br>6. **Run tests** (verify green)<br>7. Break down further into smaller pieces |

### Plan Validation

Include end-to-end validation:
- Run all tests
- Build/run application
- Verify system-level behavior
- Manual validation (if applicable)

### TDD for Unknowns

Unsure how something works? Plan investigative test first.

---

## Transitioning to Implementation

**Typically happens in NEW SESSION** (preserves context).

**Workflow**:
- Planning session: Plan → Commit → End
- **New** implementation session: Load plan → Create todos for one phase → Implement that phase

**Single-Phase Cycle** (Recommended):
- Session 1: TodoWrite Phase A tasks → Implement Phase A → Commit → End
- Session 2: TodoWrite Phase B tasks → Implement Phase B → Commit → End
- Session 3: TodoWrite Phase C tasks → Implement Phase C → Commit → End
- [Continue for as many phases as your plan has]

**Benefits**:
- Natural breakpoints every phase
- Clear "done" signal (phase complete + tests pass)
- Lower context drift risk
- Easy to resume (next session = next phase)

**If human requests implementation in same session**:
1. Convert first phase to TodoWrite todos (one phase only)
2. Update brief CURRENT MODE → implementation
3. Follow implementation mode workflow
4. Implement that phase
5. Commit phase, ask if continuing or ending session

---

## Anti-Patterns

| Pattern | Problem | Fix |
|---------|---------|-----|
| Starting implementation | Code before approval | Get explicit approval first |
| Tasks too large | "Implement entire compiler" | Break to small, focused pieces |
| No test tasks | Only implementation, no tests | Every implement needs test before it |
| Skipping validation | Ends with feature, no system check | Include end-to-end validation |
| **Not updating brief** | **Plan never written to brief** | **ALWAYS update brief's Planning Output** |
| **Not committing plan** | **Jump to implementation** | **Always commit plan before asking next step** |
| **Auto-transition** | **Start implementing after approval** | **Commit, ask what's next, WAIT** |
| **Using TodoWrite in planning** | **Skips commit step** | **TodoWrite is ONLY for implementation mode** |
| **Interpreting discussion as approval** | **"I like it" ≠ "commit it"** | **Ask: "Should I commit this plan?"** |
| Missing dependencies | Tasks have blocking issues | Map dependencies explicitly |

---

## Example Session

1. Read brief's Exploration Findings (Planning Input)
2. Load arch context (via project-docs/context/)
3. **Step 1** (Context): Read relevant source files, understand structure
4. **Step 2** (Plan): Create ATOMIC tasks across implementation phases, map dependencies
5. **Step 3** (Present):
   - "Here's the plan: [X] tasks across [Y] phases, dependencies mapped"
   - "Does this plan look good, or should I make changes?"
   - Human: "This is a lot of steps"
   - AI: "You're right. Should I: 1) Make it smaller 2) Keep it but commit for review 3) Something else?"
   - Human: "Keep it but commit for review"
   - ✅ **Explicit approval received**
6. **Step 4** (Commit):
   - Update brief's Planning Output (copy all tasks with checkboxes)
   - Commit: "Planning: Brief [number] - [title]"
   - Push to remote
   - "Plan committed and pushed. What would you like to do next?"
   - "Options: 1) Start implementation 2) End session 3) Something else"
   - **WAIT for human response**
   - **Session ends here** (or human requests implementation)

---

## Return to Exploration

**If you realize during planning**:
- Architecture decision is wrong
- Don't understand what to build
- Need to question approach
- Something feels fundamentally off

**STOP planning. Return to exploration.**

Follow exploration mode workflow to address the fundamental questions.

---

**Remember**: Design the approach, get approval, THEN implement.

---

## 🚨 CHANGE PROTOCOL FOR THIS FILE 🚨

**This file defines the planning mode process. Changing it means changing how we break down work.**

**If you think this file needs to change** (beyond example updates):

1. **STOP immediately**
2. Create change proposal document with date and description
3. Document:
   - What process you want to change
   - Why current process failed
   - What problem new process solves
   - Evidence from recent sessions
4. **DO NOT make the change**
5. Wait for human review in new session
6. Human decides if process should evolve or if drift is happening

**Better examples are fine. Process changes require review.**
