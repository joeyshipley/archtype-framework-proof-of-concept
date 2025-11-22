# BRIEF CREATION

**Purpose**: Convert human intent into structured brief through conversation.

**Note**: AI-assisted path. Briefs can also be created by humans directly or during exploration.

---

## Context Reference Map

**Foundation docs** (principles/patterns):
- `.claude/docs/README*.md` - Read at session start (already done via /start)

**User context** (if user-facing):
- `project-docs/context/personas/` - Target user personas

**Related work** (avoid duplication):
- `project-docs/briefs/` - Check for existing/related briefs

**Discovery**: Use glob for keywords when needed (e.g., `*auth*.md`, `*database*.md`)

---

## What Is This?

Brief creation converts human intent into structured work packages with clear starting mode.

---

## Workflow (5 Steps)

### 1. Understand Intent

**Ask**:
- "Who is this for?" (specific role/persona)
- "What problem are they experiencing?" (pain point)
- "What becomes possible when this exists?" (outcome)
- "Why does this matter now?" (urgency/context)
- "Is this about architecture, implementation, or a bug?" (mode hint)

**Listen for**:
- Uncertainty → Exploration needed
- Clarity → Planning/implementation ready
- Frustration → Possible mission misalignment
- User impact → Capture for Intent section

### 2. Draft Brief Sections

**Work through with human**:

| Section | Questions | Extract |
|---------|-----------|---------|
| **Intent** | "Who is this for?" "What problem?" "What outcome?" "Why now?" | User/persona, problem, outcome, urgency - NOT implementation details |
| **Constraints** | "What can't change?" "What must remain true?" | Mission alignment, arch decisions, tech constraints |
| **Signal** | "How will users know?" "What tests prove it?" "What ships?" | User signal, technical signal, outcome signal |
| **Open Questions** | "What don't we know yet?" "Architectural or implementation unknowns?" | Drives mode selection |

**Open Questions determines mode**:
- Architectural unknowns → EXPLORATION
- Implementation unknowns → PLANNING
- No unknowns → IMPLEMENTATION

### 3. Write Brief

**Create**: `project-docs/briefs/P[phase]_[category]_[feature].md`

**Use**: `.claude/docs/briefs/PRODUCT_BRIEF_TEMPLATE.md`

**Fill in**:
1. Status: "active"
2. CURRENT MODE: (exploration/planning/implementation)
3. Intent, Constraints, Signal, Open Questions
4. Leave mode sections empty (filled during work)

**Naming convention**:
- **Phase**: P000 through P999 (flexible numbering based on project size)
- **Category**: Descriptive (api, database, ui, auth, etc.)
- **Feature**: Specific work unit (kebab-case)
- **Examples**: P000_api_authentication.md, P002_database_migrations.md, P010_ui_dashboard.md

### 4. Identify Mode

**From Open Questions**:

| If | Then Mode |
|----|-----------|
| Architectural unknowns | EXPLORATION |
| Implementation unknowns | PLANNING |
| No unknowns | IMPLEMENTATION |

### 5. Tell Human Next Steps

**Template**:
> "Brief created at `project-docs/briefs/P[phase]_[category]_[feature].md`. Starting mode: [MODE].
>
> Ready to start now, or saving for later?"

**If start now**: Transition to mode

**If later**: Brief saved for new session

---

## Anti-Patterns

| Pattern | Problem | Fix |
|---------|---------|-----|
| Start implementation | "Add feature" → code immediately | Create brief first, then enter mode |
| Skip Open Questions | Pretend to know everything | Be honest about unknowns, they drive work |
| Implementation in Intent | "Create UserForm with fields X, Y, Z" | Intent is WHY not WHAT: "Enable user management" |
| Skip .claude/docs/README.md | Brief drifts from mission | Always read .claude/docs/README.md, check alignment |
| Vague constraints | Only "follow mission" | Include specific arch decisions + tech constraints |

---

## Examples

### Full Example: Vague Intent → Exploration

**Human**: "The authentication approach doesn't feel right"

**AI**: "Who needs authentication? What problem are they facing?"

**Human**: "API consumers integrating with our service. Right now there's no clear way to authenticate - some endpoints are open, some are closed, no documentation on the auth flow."

**AI**: "What should be possible when this exists?"

**Human**: "Developers should be able to securely authenticate and understand the security model immediately."

**AI**: "Why now?"

**Human**: "Blocking v1.0 launch - we can't ship without a clear auth strategy."

**AI**: "What architectural questions remain?"

**Human**: "Not sure if we need session-based or token-based auth. Where does auth live architecturally?"

**Brief**:
```
Intent:
For: API consumers integrating with our service
Problem: No clear way to authenticate - some endpoints open, some closed, no docs on auth flow
Outcome: Developers can securely authenticate and understand the security model immediately
Why Now: Blocking v1.0 launch - can't ship without auth strategy

Constraints: Must support project's user model and security requirements
Signal:
- User: "Auth flow is clear and obvious"
- Technical: All endpoints protected, auth tests pass
- Outcome: Complete auth implementation shipped
Open Questions: Session-based vs token-based? Where does auth live architecturally?
```

**Mode**: EXPLORATION (architectural unknowns)

---

### Quick Example: Planning Mode

**Open Questions**: "Token bucket vs sliding window? Redis or in-memory? How to handle distributed systems?"

→ Implementation questions (not architectural) → **PLANNING**

---

### Quick Example: Implementation Mode

**Open Questions**: None (bug clear, fix known: add null check in query)

→ No unknowns → **IMPLEMENTATION**

---

## Existing Brief

**If human references existing brief**:
1. Read brief
2. Check mode
3. Transition to mode
4. Begin work

**If needs updating**:
1. Discuss changes
2. Update sections
3. Re-evaluate mode
4. Transition

---

## Key Principles

| Principle | Why |
|-----------|-----|
| Briefs ≠ Specs | Capture intent/constraints/signal/questions. That's it. |
| Open Questions = Gold | Most valuable section. Honest unknowns drive work. |
| Mode selection matters | Wrong mode = frustration. Exploration in implementation = chaos. |
| Briefs evolve | Update as work progresses. Tool, not sacred text. |

---

## 🚨 CHANGE PROTOCOL FOR THIS FILE 🚨

**This file defines how briefs get created. Changing it means changing how humans communicate intent to AI.**

**If you think this file needs to change**:

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

**Template/structure updates are fine. Process changes require review.**

---

**Created**: 2025-10-14
**Purpose**: Guide AI through helping human create briefs
