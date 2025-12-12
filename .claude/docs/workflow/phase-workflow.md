# Phase Workflow

Each phase in a brief follows this cycle: **Research → Plan → Execute → Validate**

---

## 1. Research

**Goal:** Understand what exists before deciding how to change it.

- Read relevant source files
- Understand current implementation
- Map dependencies (what depends on what)
- Identify test coverage gaps

**Record findings in brief's Research section.** Findings inform the plan.

**If architecture feels wrong:** Stop. Discuss with human. Don't push through confusion.

---

## 2. TDD Plan

**Goal:** ATOMIC tasks that emerge from research findings.

Create tasks as TDD pairs:
```
1. [ ] Test: [specific behavior]
2. [ ] Implement: [make test pass]
3. [ ] Test: [next behavior]
4. [ ] Implement: [make test pass]
...
n. [ ] Run tests + Commit
```

**Rules:**
- Test before implement (always)
- Small, focused pairs (not batched)
- Phase ends with: Run tests → Commit
- Dependencies explicit (what blocks what)

**Get approval** before executing significant plans.

---

## 3. Execute

**Goal:** TDD cycle - red, green, repeat.

### For Each Task

1. **Write test first** - Should fail (red)
2. **Implement minimal code** - Simplest thing to pass
3. **Run test** - Should pass (green)
4. **Run full suite** - After any core file change
5. **Mark complete immediately** - No batching

### Tracking
- One task `in_progress` at a time
- Mark complete before starting next
- Update brief checkboxes at commit points

### Stop Conditions

**STOP and reassess if:**
- Bug discovered during feature work
- Architecture feels fundamentally wrong
- Plan isn't working
- Confusion compounds

Don't push through. Stop, investigate, adjust.

---

## 4. Validate

**Goal:** Confirm the phase achieved its goal.

- [ ] All tests passing
- [ ] Build succeeds
- [ ] Phase goal achieved
- [ ] Brief updated (Done section)
- [ ] Committed

**Then:** Move to next phase or end session.

---

## TDD Investigation (Bug Location Unknown)

When unsure WHERE a bug is:

1. Write test for what SHOULD work
2. Run test - pass or fail?
3. Analyze:
   - Passes → Bug in data flow (test data ≠ production)
   - Fails → Bug in logic (implementation wrong)
4. Fix based on evidence

---

## Quick Reference

| Step | Output | Record In |
|------|--------|-----------|
| Research | Understanding + findings | Brief: Research section |
| Plan | ATOMIC TDD tasks | Brief: TDD Plan section |
| Execute | Working code | TodoWrite + Brief checkboxes |
| Validate | Passing tests + commit | Brief: Done section |

**The cycle repeats for each phase in the brief.**
