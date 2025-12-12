# Brief Creation

**Purpose:** Convert human intent into structured brief through conversation.

---

## Workflow

### 1. Understand Intent

Ask these questions to extract the brief invariants:

| Question | Maps To |
|----------|---------|
| "What are you building or changing?" | Goal |
| "What's broken or missing today?" | Problem |
| "How will you know it works?" | Success Signal |
| "What can't change?" | Constraints |

**Listen for:**
- Uncertainty → capture in Open Questions
- Clarity → ready for phase breakdown
- User impact → capture in Problem/Success Signal

### 2. Capture Unknowns

Ask: "What don't we know yet?"

Record in Open Questions. These drive the research in each phase.

### 3. Break into Phases

Work with human to identify logical chunks:
- Each phase has a clear goal (what it achieves)
- Phases are sequential - later phases may depend on earlier findings
- Don't over-plan - 2-4 phases is usually enough to start

### 4. Create Brief

**File:** `project-docs/briefs/[category]_[feature].md`

**Template:** `.claude/docs/briefs/BRIEF_TEMPLATE.md`

**Fill in:**
- Goal, Problem, Success Signal, Constraints (invariants)
- Phase table with goals
- Open Questions
- Leave per-phase sections empty (filled during work)

### 5. Next Steps

Tell human:
> "Brief created at `project-docs/briefs/[name].md`. Ready to start Phase 1 research, or saving for later?"

**If start now:** Begin Phase 1 research using phase-workflow.md
**If later:** Brief saved for new session

---

## Example

**Human:** "The error messages are confusing users"

**AI asks:**
- "What are you building?" → "Clear, actionable error messages"
- "What's broken?" → "Users don't understand what went wrong or how to fix it"
- "How will you know it works?" → "Users can self-resolve common errors without support"
- "What can't change?" → "Must use existing error handling infrastructure"

**Phases:**
1. Audit current error messages
2. Design error message patterns
3. Implement new messages

**Brief created, start Phase 1 research.**

---

## Existing Brief

If human references existing brief:
1. Read brief
2. Find current phase (first incomplete phase)
3. Resume work using phase-workflow.md
