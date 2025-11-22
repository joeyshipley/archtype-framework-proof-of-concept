---
---

# PERSONA CREATION

**Purpose**: Convert user observations into structured persona through conversation.

**Note**: AI-assisted path. Personas can also be created by humans directly using template.

---

## What Is This?

**Persona creation converts**:
- User observations → Structured persona
- Real-world behavior → Design target
- "Users want X" → Specific persona with context

**Not for**:
- Creating fictional users without evidence
- Making feature decisions
- Defining requirements

---

## Workflow (5 Steps)

### 1. Gather Observations

**Ask**:
- "Who is this person? What role do they play?"
- "What have you observed about their behavior?"
- "What are their goals when using the platform?"
- "What frustrates them or blocks them?"
- "What's their technical skill level?"

**Listen for**:
- Specific behaviors (not assumptions)
- Real pain points (not hypothetical)
- Actual goals (not what you wish they wanted)
- Context (age, environment, constraints)

### 2. Identify Category

**Which type?**
- **Reader** - Consumes content on platform
- **Author** - Creates content for platform
- **Admin** - Manages/moderates platform
- **Parent** - Gatekeeps access for children

### 3. Draft Persona Sections

**Work through with human**:

| Section | Questions | Extract |
|---------|-----------|---------|
| **Demographics** | "How old? Where? Tech level?" | Age, location, tech savvy, relevant skill level |
| **Background** | "What's their context?" | 2-4 bullets about experience, interests, constraints |
| **Goals** | "What do they want to accomplish?" | Specific outcomes they're trying to achieve |
| **Pain Points** | "What frustrates or blocks them?" | Real problems they experience |
| **Behaviors** | "How do they actually use it?" | Observable actions and patterns |
| **Needs** | "What must the platform provide?" | Requirements to serve this persona |
| **Quote** | "One sentence in their voice?" | Captures their perspective authentically |

### 4. Choose File Name

**Naming convention**: `[role].md` (NOT person's name)

**Examples**:
- `eager-explorer.md` (NOT `emma.md`)
- `aspiring-novelist.md` (NOT `rachel.md`)
- `concerned-parent.md` (NOT `jennifer.md`)

**Why?** Names can change as personas evolve/split/merge. Role is stable.

### 5. Write Persona File

**Create**: `project-docs/context/personas/[role].md`

**Use**: `project-docs/context/personas/PERSONA_TEMPLATE.md`

**Include**:
- Real name as heading (can change later)
- All sections filled with specific details
- Quote that captures their voice

**Update**: `project-docs/context/personas/README.md` with new persona entry

---

## Anti-Patterns

| Pattern | Problem | Fix |
|---------|---------|-----|
| Fictional personas | Made-up users without evidence | Base on real observations/research |
| Too generic | "Users want it to work" | Specific goals, behaviors, pain points |
| Assumption-driven | "They probably want..." | Use actual observations |
| Feature-focused | "User who needs feature X" | Focus on goals/problems, not solutions |
| Skip behaviors | Only demographics + goals | Behaviors reveal how to design for them |

---

## Example Flow

**Human**: "I interviewed a teacher who wants to write stories for her students."

**AI**: "Tell me more about her. What's her technical skill level?"

**Human**: "Low. She uses Word and email but that's about it. She's intimidated by anything technical."

**AI**: "What are her goals with the platform?"

**Human**: "She wants to share stories with young readers and get validation. She dreams of being published but traditional route intimidates her."

**AI**: "What frustrates her about creating interactive stories?"

**Human**: "She doesn't understand game mechanics or coding. She has great story ideas but doesn't know how to structure them for this format."

**AI**: "How does she work? What are her patterns?"

**Human**: "She writes in evenings and weekends. Needs step-by-step guidance. Prefers examples over documentation."

**AI**: "This sounds like an Author persona. Should we call the role something like 'aspiring-novelist'?"

**Human**: "Perfect."

**AI creates**: `project-docs/context/personas/aspiring-novelist.md` with all details structured using template.

---

## When to Create New Persona

**Create new persona when**:
- Distinct behavior patterns emerge
- Different goals/motivations than existing personas
- Unique pain points or needs
- Different technical level or context

**Don't create when**:
- Minor variation of existing persona
- Same goals/behaviors, just different demographics
- Hypothetical "what if" user without evidence

**Instead**: Update existing persona or add variation notes.

---

## Key Principles

| Principle | Why |
|-----------|-----|
| Evidence-based | Personas represent real users, not wishful thinking |
| Specific details | "11yo suburban reader" > "young user" |
| Behavior over demographics | How they act matters more than who they are |
| Quote captures voice | One sentence should feel authentic |
| Role-based naming | Names change, roles stay stable |

---

## 🚨 CHANGE PROTOCOL FOR THIS FILE 🚨

**This file defines how personas get created. Changing it means changing how we understand users.**

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

**Created**: 2025-11-22
**Purpose**: Guide AI through helping human create personas
