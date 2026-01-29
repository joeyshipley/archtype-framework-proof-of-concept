# ArchType Architecture

A visual guide to how requests flow through the framework.

---

## Components At a Glance

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                                 BROWSER                                     │
│  ┌─────────────────────────────────────────────────────────────────────┐    │
│  │  <div id="todo-page" data-view="TodosPage" data-domain="todosList"> │    │
│  │       ↑                    ↑                      ↑                 │    │
│  │    DOM ID            View Class Name         Domain Name            │    │
│  │  (for swap)         (for re-render)      (for mutation match)       │    │
│  └─────────────────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │
                    ┌───────────────┴───────────────┐
                    ▼                               ▼
              GET /todos                  POST /interaction/todos/create
            (Page Load)                        (Mutation)
```

---

## Component Responsibilities

```
┌──────────────────┐     ┌──────────────────┐     ┌──────────────────┐
│      VIEW        │     │    PROVIDER      │     │   PERFORMER      │
│                  │     │                  │     │                  │
│  "What data do   │     │  "How to fetch   │     │  "How to change  │
│   I need?"       │     │   that data"     │     │   data"          │
│                  │     │                  │     │                  │
│  Dependencies =  │     │  FetchTyped() →  │     │  Perform() →     │
│  From<TDomain>() │     │  DomainView      │     │  Validate,       │
│                  │     │                  │     │  Execute, Save   │
│  Render(data) →  │     │                  │     │                  │
│  HTML            │     │                  │     │                  │
└──────────────────┘     └──────────────────┘     └──────────────────┘
        │                        │                        │
        │                        │                        │
        ▼                        ▼                        ▼
┌──────────────────┐     ┌──────────────────┐     ┌──────────────────┐
│   DOMAIN VIEW    │     │   REPOSITORY     │     │  SPECIFICATION   │
│                  │     │                  │     │                  │
│  Data shape for  │     │  Database        │     │  Query criteria  │
│  the view        │     │  operations      │     │  as object       │
│                  │     │                  │     │                  │
│  DomainName =    │     │  List(spec)      │     │  Todo.ByUserId() │
│  "todosList"     │     │  Get(spec)       │     │  Todo.ById()     │
│                  │     │  Add(), Delete() │     │                  │
└──────────────────┘     └──────────────────┘     └──────────────────┘
```

---

## Initial Page Load

```
    Browser                    Server
       │                         │
       │  GET /todos             │
       │────────────────────────▶│
       │                         │
       │                    ┌────┴────┐
       │                    │  Route  │
       │                    │ Handler │
       │                    └────┬────┘
       │                         │
       │                         ▼
       │                ┌─────────────────┐
       │                │ TodosPage       │
       │                │ .Dependencies = │
       │                │ From<TodosList  │
       │                │    DomainView>()│
       │                └────────┬────────┘
       │                         │
       │                         ▼
       │                ┌─────────────────┐
       │                │  DataLoader     │
       │                │                 │
       │                │  Find provider  │
       │                │  for domain     │
       │                └────────┬────────┘
       │                         │
       │                         ▼
       │                ┌─────────────────┐
       │                │ TodosListProv.  │
       │                │                 │
       │                │ FetchTyped()    │
       │                └────────┬────────┘
       │                         │
       │                         ▼
       │                ┌─────────────────┐
       │                │  Repository     │
       │                │                 │
       │                │ .List(Todo      │
       │                │   .ByUserId())  │
       │                └────────┬────────┘
       │                         │
       │                         ▼
       │                    ┌─────────┐
       │                    │   DB    │
       │                    └────┬────┘
       │                         │
       │                         ▼
       │                ┌─────────────────┐
       │                │ TodosListDomain │
       │                │ View            │
       │                │ {               │
       │                │   List: [...],  │
       │                │   OpenCount: 3  │
       │                │ }               │
       │                └────────┬────────┘
       │                         │
       │                         ▼
       │                ┌─────────────────┐
       │                │ TodosPage       │
       │                │ .Render(data)   │
       │                │                 │
       │                │ → HTML with     │
       │                │   data-view     │
       │                │   data-domain   │
       │                └────────┬────────┘
       │                         │
       │  200 OK                 │
       │  Full HTML Page         │
       │◀────────────────────────│
       │                         │
```

---

## Mutation Flow (Form Submit)

```
    Browser                         Server
       │                              │
       │  ┌────────────────────┐      │
       │  │ X-Component-Context│      │
       │  │ header contains:   │      │
       │  │ [{                 │      │
       │  │   id: "todo-page", │      │
       │  │   viewType:        │      │
       │  │    "TodosPage",    │      │
       │  │   domain:          │      │
       │  │    "todosList"     │      │
       │  │ }]                 │      │
       │  └────────────────────┘      │
       │                              │
       │  POST /interaction/          │
       │       todos/create           │
       │  + { title: "Buy milk" }     │
       │  + X-Component-Context       │
       │─────────────────────────────▶│
       │                              │
       │                         ┌────┴────┐
       │                         │ Create  │
       │                         │ Todo    │
       │                         │ Inter.  │
       │                         └────┬────┘
       │                              │
       │                              │ Mutates =
       │                              │ DataMutations.For("todosList")
       │                              │
       │                              ▼
       │                    ┌──────────────────┐
       │                    │  [FromForm]      │
       │                    │  Model Binding   │
       │                    │                  │
       │                    │  CreateTodo      │
       │                    │  Request {       │
       │                    │    Title =       │
       │                    │    "Buy milk"    │
       │                    │  }               │
       │                    └────────┬─────────┘
       │                             │
       │                             ▼
       │                    ┌──────────────────┐
       │                    │ CreateTodo       │
       │                    │ Performer        │
       │                    │                  │
       │                    │ 1. Validate      │
       │                    │ 2. Create Todo   │
       │                    │ 3. Repository    │
       │                    │    .Add()        │
       │                    │ 4. SaveChanges() │
       │                    └────────┬─────────┘
       │                             │
       │                             ▼
       │                    ┌──────────────────┐
       │                    │  OnSuccess()     │
       │                    │                  │
       │                    │  BuildOobResult()│
       │                    └────────┬─────────┘
       │                             │
       │                             ▼
       │                    ┌──────────────────┐
       │                    │  Framework       │
       │                    │  Orchestrator    │
       │                    │                  │
       │                    │  1. Parse header │
       │                    │  2. Match domain │
       │                    │     "todosList"  │
       │                    │  3. Re-fetch     │
       │                    │     domain data  │
       │                    │  4. Re-render    │
       │                    │     TodosPage    │
       │                    │  5. Inject       │
       │                    │     hx-swap-oob  │
       │                    └────────┬─────────┘
       │                             │
       │  200 OK                     │
       │  OOB HTML fragments         │
       │◀────────────────────────────│
       │                             │
       │  ┌────────────────────┐     │
       │  │ HTMX processes:    │     │
       │  │                    │     │
       │  │ <div id="todo-page"│     │
       │  │  hx-swap-oob="true"│     │
       │  │  data-view="..."   │     │
       │  │  data-domain="...">│     │
       │  │   (fresh content)  │     │
       │  │ </div>             │     │
       │  │                    │     │
       │  │ Swaps by ID ───────┼──▶ DOM Updated
       │  └────────────────────┘     │
       │                             │
```

---

## How OOB Updates Work

### Step 1: Page Renders with Metadata
```html
<div id="todo-page"
     data-view="TodosPage"
     data-domain="todosList">
  <!-- content -->
</div>
```

### Step 2: Client JS Collects All Views
```javascript
// component-context.js (runs on every HTMX request)
const views = document.querySelectorAll('[data-view]');
const context = views.map(el => ({
    id: el.id,                    // "todo-page"
    viewType: el.dataset.view,    // "TodosPage"
    domain: el.dataset.domain     // "todosList"
}));
// Sent as X-Component-Context header
```

### Step 3: Interaction Declares Mutations
```csharp
// In CreateTodoInteraction
protected override DataMutations Mutates =>
    DataMutations.For(TodosListDomainView.DomainName);
    //            └─── "todosList"
```

### Step 4: Framework Matches & Re-renders
```
X-Component-Context:     Interaction Mutates:
┌─────────────────┐      ┌─────────────────┐
│ TodosPage       │      │ "todosList"     │
│ domain:         │      │                 │
│  "todosList" ◀──┼──────┼─── MATCH! ──────│
└─────────────────┘      └─────────────────┘
        │
        ▼
   Re-render TodosPage
   with fresh data
```

### Step 5: HTMX Swaps by ID
```html
<!-- Server returns: -->
<div id="todo-page" hx-swap-oob="true" data-view="TodosPage" data-domain="todosList">
  <!-- NEW content with new todo -->
</div>

<!-- HTMX finds id="todo-page" in DOM and replaces it -->
```

---

## Data Binding

### Form Fields → Request Properties

```
┌─────────────────────────────────────────────────────────────────┐
│                         HTML FORM                               │
│                                                                 │
│   <form hx-post="/interaction/todos/create">                    │
│       <input name="title" value="Buy milk" />                   │
│   </form>                        │                              │
│                                  │                              │
└──────────────────────────────────┼──────────────────────────────┘
                                   │
                                   │ name="title" matches
                                   │ property name
                                   ▼
┌─────────────────────────────────────────────────────────────────┐
│                     REQUEST OBJECT                              │
│                                                                 │
│   public class CreateTodoRequest                                │
│   {                                                             │
│       public string Title { get; set; }  ◀── "Buy milk"         │
│   }                                                             │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### Button with ModelId → hx-vals

```
┌─────────────────────────────────────────────────────────────────┐
│                        C# ELEMENT                               │
│                                                                 │
│   new Button("Toggle")                                          │
│       .Action("/interaction/todos/toggle")                      │
│       .ModelId(todo.Id)   // Id = 123                           │
│                                                                 │
└──────────────────────────────────┬──────────────────────────────┘
                                   │
                                   │ Renders as
                                   ▼
┌─────────────────────────────────────────────────────────────────┐
│                         HTML OUTPUT                             │
│                                                                 │
│   <button hx-post="/interaction/todos/toggle"                   │
│           hx-vals='{"id": 123}'>                                │
│       Toggle                                                    │
│   </button>                                                     │
│                                                                 │
└──────────────────────────────────┬──────────────────────────────┘
                                   │
                                   │ HTMX includes in POST
                                   ▼
┌─────────────────────────────────────────────────────────────────┐
│                     REQUEST OBJECT                              │
│                                                                 │
│   public class ToggleTodoRequest                                │
│   {                                                             │
│       public long Id { get; set; }  ◀── 123                     │
│   }                                                             │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## File Organization

```
PagePlay.Site/
│
├── Pages/
│   └── Todos/
│       ├── Todos.Page.cs              ◀── VIEW (declares dependencies, renders HTML)
│       ├── Todos.Route.cs             ◀── ROUTE (GET /todos)
│       └── Interactions/
│           ├── CreateTodo.Interaction.cs   ◀── INTERACTION (POST handler)
│           └── ToggleTodo.Interaction.cs
│
├── Application/
│   └── Todos/
│       ├── Models/
│       │   └── Todo.cs                ◀── ENTITY + SPECIFICATIONS
│       │
│       ├── Perspectives/
│       │   └── List/
│       │       ├── TodosList.DomainView.cs  ◀── DOMAIN VIEW (data shape)
│       │       └── TodosList.Provider.cs    ◀── PROVIDER (fetches data)
│       │
│       └── Performers/
│           ├── CreateTodo/
│           │   ├── CreateTodo.Performer.cs      ◀── PERFORMER (business logic)
│           │   └── CreateTodo.BoundaryContracts.cs  ◀── REQUEST/RESPONSE
│           └── ToggleTodo/
│               └── ...
│
└── Infrastructure/
    ├── Web/
    │   ├── Framework/
    │   │   └── FrameworkOrchestrator.cs    ◀── ORCHESTRATOR (coordinates everything)
    │   ├── Data/
    │   │   └── DataLoader.cs               ◀── DATA LOADER (parallel fetch)
    │   └── Pages/
    │       └── PageInteractionBase.cs      ◀── BASE INTERACTION CLASS
    │
    └── Data/
        ├── Repositories/
        │   └── Repository.cs               ◀── REPOSITORY (database access)
        └── Specifications/
            └── Specification.cs            ◀── SPECIFICATION (query builder)
```

---

## The Declarative Loop

```
┌─────────────────────────────────────────────────────────────────────────┐
│                                                                         │
│    ┌──────────────┐         ┌──────────────┐         ┌──────────────┐   │
│    │    VIEW      │         │  FRAMEWORK   │         │ INTERACTION  │   │
│    │              │         │              │         │              │   │
│    │ "I need      │────────▶│ Tags HTML    │         │ "I mutated   │   │
│    │  todosList   │         │ with domain  │         │  todosList"  │   │
│    │  domain"     │         │              │         │              │   │
│    │              │         │              │◀────────│              │   │
│    └──────────────┘         └──────────────┘         └──────────────┘   │
│           ▲                        │                        │           │
│           │                        │                        │           │
│           │                        ▼                        │           │
│           │                 ┌──────────────┐                │           │
│           │                 │   CLIENT     │                │           │
│           │                 │              │                │           │
│           │                 │ Tracks all   │                │           │
│           │                 │ data-domain  │────────────────┘           │
│           │                 │ on page      │                            │
│           │                 │              │  (X-Component-Context)     │
│           │                 └──────────────┘                            │
│           │                        │                                    │
│           │                        │                                    │
│           └────────────────────────┘                                    │
│                    Framework matches                                    │
│                    mutation → views                                     │
│                    and re-renders                                       │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘

No manual wiring. Declarations drive everything.
```
