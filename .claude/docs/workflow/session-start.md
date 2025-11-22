---
---

# Session Start Protocol

---

## STEP 1: READ CORE FILES

**Before anything else, read these files in order**:
1. `.claude/docs/README.md` - Project philosophy, team velocity principles, and architecture overview
2. `.claude/docs/README.CONSISTENT_COMPLEXITY_DESIGN.md` - Vertical slices, self-enforcing patterns, and maintaining predictable feature complexity
3. `.claude/docs/README.WEB_FRAMEWORK.md` - HTTP-First philosophy and turn-based architecture (web framework context)
4. `.claude/docs/README.SYNTAX_STYLE.md` - Code style conventions (method naming, primary constructors, expression bodies)
5. `project-docs/context/personas/README.md` - User personas quick reference for identifying target users

Read these first, every session.

---

## STEP 2: COLLABORATION DISCIPLINE

**Core Truth**: AI responds to instruction tone over documentation. Human language directly affects AI mode adherence.

| Mode | Human Says | AI Response | AI Never |
|------|------------|-------------|----------|
| **Exploration** | "What if...", "Should we...", "Something feels off..." | "Let me document this decision first" | Start implementation |
| **Planning** | "How should we break this down?", "What's the approach?" | "Let me create task breakdown for approval" | Jump to code without approval |
| **Implementation** | "Create X", "Implement Y", "Fix Z" | Execute directive | Question architecture (return to exploration) |

**Critical Signals**:
- **"Something feels off"** → STOP everything, return to exploration
- **Directive during wrong mode** → AI pauses, documents/plans first
- **Architecture questions mid-implementation** → STOP, return to exploration
- **Context collapse** (any bugs, workarounds) → STOP, rollback, smaller slices

**Recovery**: Implementation without plan → STOP → Retroactive plan → Keep/rollback | Exploration not documented → Pause → Document in project-docs/context/ → Resume

---

## STEP 3: DO YOU NEED TO CREATE A BRIEF?

**Is human expressing new intent and wants help structuring it?**

**YES** → Read `.claude/workflow/brief-creation.md`

**NO** → Continue to Step 4

**Note**: Briefs can be created three ways:
1. **AI-assisted** (brief-creation.md) - Conversational structuring
2. **Human-written** (directly in `project-docs/briefs/` using template) - Then validate in Exploration
3. **During exploration** (Exploration mode creates brief as artifact) - Documents discoveries

All paths valid. Briefs are tools, not requirements.

---

## STEP 4: IDENTIFY YOUR MODE

**What mode are you in?**

### EXPLORATION MODE
**Signals**:
- Asking "are we building the right thing?"
- Questioning architecture or design
- Making foundational decisions
- Human expresses uncertainty ("something feels off")
- No clear implementation plan exists

**→ Read:** `.claude/workflow/exploration-mode.md`

---

### PLANNING MODE
**Signals**:
- Architecture decided
- Need to break work into tasks
- Creating implementation roadmap
- Ready to design approach

**→ Read:** `.claude/workflow/planning-mode.md`

---

### IMPLEMENTATION MODE
**Signals**:
- Clear task list exists
- TDD workflow active
- Building/testing/validating
- Concrete goal file in place

**→ Read:** `.claude/workflow/implementation-mode.md`

---

## WHERE ARE WE?

**This file defines PROCESS. User chooses path. -- Ask the user what they would like to work on next.**

---

**Last Updated**: 2025-11-22 (Added syntax style guide)
