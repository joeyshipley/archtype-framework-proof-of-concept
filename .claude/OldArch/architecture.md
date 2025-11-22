# Architecture & Design Philosophy

## Overview

TurnBasedTodo is a server-centric, hypermedia-driven web application that embraces the turn-based game metaphor for task management. Every user action is conceptualized as a "turn" in a game where the server acts as the game master.

## Core Beliefs

### 1. Server Authority
The server is the single source of truth. All business logic, state management, and data validation occur server-side. This approach provides:
- Simplified security model (all logic behind the server boundary)
- Consistent validation and business rules
- Easier debugging and testing
- Natural protection against client-side tampering

### 2. Hypermedia as the Engine of Application State (HATEOAS)
Rather than treating the server as a JSON API, we embrace hypermedia. The server responds with HTML fragments that describe both data and available actions. HTMX enables this pattern with modern UX expectations.

### 3. Minimal Client Complexity
We deliberately avoid client-side JavaScript frameworks and state management. The only JavaScript in the application is:
- HTMX library (declarative, loaded via CDN)
- Small inline event handlers for form resets and event triggering

This keeps the codebase simple, maintainable, and accessible.

### 4. Progressive Enhancement
The application is built on standard web technologies (HTML forms, HTTP methods). HTMX enhances the experience but the underlying mechanics are traditional web patterns.

## Technology Stack

### Backend
- **.NET 10.0** - Latest .NET runtime
- **ASP.NET Core MVC** - Controller-based web framework
- **Entity Framework Core 9.0** - ORM for database access
- **PostgreSQL 16** - Relational database
- **Razor Views** - Server-side templating

### Frontend
- **HTMX 1.9** - Declarative AJAX and hypermedia interactions
- **Inline CSS** - Scoped styling within views
- **No build process** - No bundlers, transpilers, or compilation steps

### Infrastructure
- **Docker Compose** - PostgreSQL container orchestration
- **EF Core Migrations** - Database schema versioning

## Architectural Patterns

### Turn-Based Metaphor
Every endpoint is prefixed with `/turn/*` (except the root). This naming convention reinforces the conceptual model:
- User actions are "turns"
- Server processes each turn
- Each turn results in an updated game state

Examples:
- `POST /turn/add` - Take a turn to add a todo
- `PUT /turn/toggle/{id}` - Take a turn to toggle completion
- `GET /turn/edit/{id}` - Enter edit mode for a turn

### Partial View Components
The UI is decomposed into reusable partial views, each responsible for rendering a specific component:

- **Board.cshtml** - Main layout and game shell
- **_TodoList.cshtml** - Collection of todos with empty state handling
- **_TodoItem.cshtml** - Individual todo in view mode
- **_TodoEditForm.cshtml** - Individual todo in edit mode
- **_Stats.cshtml** - Statistics panel

This componentization enables granular HTML fragment updates via HTMX.

### State Machines via View Swapping
Todos can exist in two states: view mode and edit mode. HTMX's `hx-swap="outerHTML"` enables seamless transitions:

```
View Mode (_TodoItem)
    ↓ (Click Edit)
Edit Mode (_TodoEditForm)
    ↓ (Save)         ↓ (Cancel)
View Mode            View Mode
```

The server decides which view to return based on the action, creating a simple state machine.

### Event-Driven Reactive Updates
Custom HTMX events coordinate cross-component updates:

```javascript
hx-on::after-request="htmx.trigger('body', 'turnComplete')"
```

When any turn completes, this event fires. The stats panel listens:

```html
hx-trigger="load, turnComplete from:body"
```

This creates reactive behavior without client-side state management.

## Data Flow

```
User Interaction (Click/Submit)
    ↓
HTMX Intercepts (Browser)
    ↓
HTTP Request → /turn/* Endpoint
    ↓
GameController Action
    ↓
Entity Framework Core
    ↓
PostgreSQL Database
    ↓
EF Returns Entities
    ↓
Controller → Partial View
    ↓
Razor Renders HTML Fragment
    ↓
HTTP Response (HTML)
    ↓
HTMX Swaps into DOM
    ↓
Triggers Custom Events (if needed)
```

## Boundary Responsibilities

### Controller Boundary
- Route handling and HTTP method mapping
- Orchestrating database operations
- Selecting appropriate views based on action context
- Managing state transitions (view ↔ edit mode)

Pattern: `Receive Turn → Query/Modify DB → Return Partial View`

### Data Access Boundary
- Entity Framework Core `DbContext`
- Single `DbSet<Todo>` exposing todos table
- No additional repository abstraction (EF Core provides sufficient abstraction)

### Domain Model Boundary
- Simple data structures with validation attributes
- Anemic domain model (appropriate for this application scale)
- Data annotations for validation (`[Required]`, `[StringLength]`)

### View Boundary
- HTML structure generation
- HTMX attribute configuration (defines client behavior)
- Model binding via Razor
- Presentation logic only

### Infrastructure Boundary
- Dependency injection setup
- Middleware pipeline configuration
- Database connection management
- Session infrastructure (configured for future use)

## Design Tradeoffs

### Advantages
- **Simplicity** - No build process, minimal dependencies
- **Security** - Logic server-side, smaller attack surface
- **Development Speed** - No API contracts, no serialization layers
- **Maintainability** - Single programming paradigm (C#/Razor)
- **SEO-Friendly** - Server renders real HTML
- **Accessibility Baseline** - Standard HTML forms work without JavaScript

### Tradeoffs
- **Network Chattiness** - Every interaction requires server round-trip
- **Server Load** - Server renders all UI updates
- **No Offline Support** - Requires active server connection
- **Limited Client Interactivity** - Complex animations/interactions harder
- **Scaling Considerations** - Server-side rendering per request

## Why This Architecture?

This architecture is a deliberate return to server-centric design patterns, enhanced with modern tooling (HTMX) that provides SPA-like UX without SPA complexity. It's appropriate when:

- Team expertise is backend-focused
- Application logic is inherently server-side
- Simplicity and maintainability are prioritized
- Real-time collaboration isn't required
- SEO and accessibility matter

This is **not** jQuery-era development disguised as modern architecture. It's a thoughtful application of hypermedia principles enabled by HTMX, representing a viable alternative to the SPA-dominated frontend landscape.

## Future Considerations

The architecture supports natural evolution paths:
- Add real-time features via SignalR
- Introduce client-side JavaScript only where DOM manipulation is essential
- Scale horizontally with stateless server design
- Migrate to API-driven architecture if requirements change

The simple foundation makes pivoting easier than untangling a complex SPA would be.
