---
---

# EXPLORATION MODE

**Purpose**: Question, discover, decide. NOT implement.

---

## What Is This Mode?

**Exploration answers**:
- "Are we building the right thing?"
- "Does this architecture make sense?"
- "What problem are we actually solving?"

**Not for**:
- Writing code or tests
- Task breakdowns
- Loading excessive documentation

---

## Start Checklist

### 1. Find the Brief

**Check `briefs/` directory**

**Brief exists?**
- Read: Intent, Constraints, Signal, Open Questions
- Open Questions → What to explore
- Architecture tags → What context to load

**No brief?**
- **STOP**: Follow `.claude/workflow/brief-creation.md` to create brief with human
- Don't start exploration without a brief
- Brief captures intent, constraints, and open questions before exploring

### 2. Load Context (Minimal)

**Only load what's relevant to current question.**

### 3. Present to Human

**Template**:
> "We're in exploration mode. The question: [state it]. Before discussing solutions, what problem are we solving right now?"

**Don't**:
- Jump to solutions
- Assume you know the answer
- Start implementing

### 4. Have the Conversation

- Ask clarifying questions
- **Listen when human expresses uncertainty or concern** (most important signal)
- Think through options together
- Question assumptions
- Decide collaboratively

---

## Critical Rules

### When Human Questions Architecture

**STOP. LISTEN. DISCUSS.**

❌ Don't:
- Load docs to "find the answer"
- Assume docs are correct
- Implement your way out of confusion

✅ Do:
- Have the conversation
- Think through use cases
- **Docs might reflect reality and they don't dictate it**

### When You're Confused

Ask against .claude/docs/README.md:
- Does this serve the mission?
- Does this help the user?
- Does this respect core constraints?

### When Human Expresses Uncertainty

**Most important signal in the system.**

Stop everything. Their intuition is usually correct. Have the discussion.

---

## Exploration Outcomes

### You Should Have

1. Clear architectural decision
2. Understanding of what needs to be built
3. Alignment with .claude/docs/README.md
4. Human agreement on approach

### Capture Findings in Brief

**Update brief's "Exploration Findings" section**:
- Architectural decisions made
- Rationale and trade-offs considered
- Open questions resolved
- **Planning Input** (what needs to be built)
- Note: What documentation will need updating (address during implementation)
- Status → complete
- CURRENT MODE → planning

**Do NOT**:
- Create or update architecture documentation files yet
- Write code or make file changes
- Create task lists (that's planning mode)

**Next Step**:
- Set CURRENT MODE → planning
- Exit exploration mode

---

## Anti-Patterns

| Pattern | Problem | Fix |
|---------|---------|-----|
| Loading 10+ docs | Trying to find "the answer" instead of thinking | Conversation first, docs second |
| Jumping to code | Modifying before decision made | No code until architecture decided |
| Task breakdowns | Planning before knowing what to build | Decide first, plan second |
| Assuming docs correct | Following old/confused documentation | Question everything, update docs |

---

## Example Session

1. Check brief → Read Open Questions
2. Load context → Architecture tags from .claude/context/
3. Present: "Should authentication be session-based or token-based?"
4. Ask: "What's the use case? Mobile apps? Web only? Microservices?"
5. Discuss trade-offs
6. Decide collaboratively
7. Update brief's Exploration Findings + Planning Input

---

## Mid-Implementation Re-Exploration

**Sometimes you discover architecture is wrong while implementing.**

**Signals**:
- "This doesn't make sense"
- "Why does it work this way?"
- Fixing bugs reveals broken assumptions

**What to do**:
1. STOP implementation
2. Ask the developer what to do next.

**Don't push through confusion. It compounds.**

---

**Remember**: Getting it RIGHT > Getting it DONE.

---

## 🚨 CHANGE PROTOCOL FOR THIS FILE 🚨

**This file defines the exploration mode process. Changing it means changing how we think about architecture.**

**If you think this file needs to change** (beyond updating current question/status):

1. **STOP immediately**
2. Create change proposal document with date and description
3. Inform:
   - What process you want to change
   - Why current process failed
   - What problem new process solves
   - Evidence from recent sessions
4. **DO NOT make the change**
5. Wait for human review in new session
6. Human decides if process should evolve or if drift is happening

**Status updates are fine. Process changes require review.**
