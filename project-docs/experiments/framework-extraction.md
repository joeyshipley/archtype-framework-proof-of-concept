# Experiment: Framework Extraction to ArchType.Framework

**Status:** In Progress
**Started:** 2025-12-12
**Goal:** Extract generic framework code from PagePlay.Site/Infrastructure into ArchType.Framework, leaving only application-specific infrastructure in PagePlay.Site

---

## Overview

### Problem Statement

The current `PagePlay.Site/Infrastructure` folder contains a mix of:
1. **Framework code** - Generic, reusable abstractions that any application built on this architecture would need (IPerformer, IView, Repository, etc.)
2. **Application infrastructure** - PagePlay-specific implementations (AppDbContext, migrations, specific auth config)
3. **Gray area** - Code that could go either way depending on how we want to structure the framework

### Goal

Create a clean separation where:
- `ArchType.Framework` contains all generic framework abstractions and base implementations
- `PagePlay.Site` references the framework and contains only application-specific code
- The framework is reusable for building other applications with this architecture

### Success Criteria

1. ArchType.Framework compiles independently
2. PagePlay.Site references ArchType.Framework
3. Framework contains no PagePlay-specific code (no entity references, no app config)
4. All existing functionality preserved (app still works)
5. Clear documentation of what belongs where

---

## Infrastructure Inventory

### Summary by Area (After Deep Review)

| Area | Total Files | Framework | App-Specific | Gray Area | Delete |
|------|-------------|-----------|--------------|-----------|--------|
| Core/Application | 12 | 10 | 1 | 1 | 0 |
| Core/Domain | 2 | 2 | 0 | 0 | 0 |
| Data | 14 | 4 | 9 | 1 | 1 |
| Security | 5 | 5 | 0 | 0 | 0 |
| Web | 24 | 22 | 0 | 2 | 0 |
| UI | 13 | 12 | 1 | 0 | 0 |
| Dependencies | 2 | 1 | 1 | 0 | 0 |
| **Total** | **72** | **56** | **12** | **4** | **1** |

**Key Findings:**
- 78% of Infrastructure is framework code (56 files)
- Only 17% is app-specific (12 files: migrations, DbContext, DependencyResolver, warmup, theme)
- 4 files need modification for framework (assembly discovery, Repository generics)
- 1 file should be deleted (empty ICacheProvider placeholder)

---

## Detailed Review by Area

### Infrastructure/Core/Application

| File | Classification | Notes | Reviewed |
|------|----------------|-------|----------|
| `IPerformer.cs` | Framework | Core abstraction: `IPerformer<TRequest, TResponse>` with `Perform()` method | ✅ |
| `IPerformerRequest.cs` | Framework | Empty marker interface for type constraints | ✅ |
| `IPerformerResponse.cs` | Framework | Empty marker interface for type constraints | ✅ |
| `PerformerBase.cs` | Framework | Convenience base: `Fail(ValidationResult)`, `Fail(string)`, `Succeed(TResponse)` | ✅ |
| `ApplicationResult.cs` | Framework | Contains both interface and impl: `IApplicationResult<T>` + `ApplicationResult<T>` with Success, Errors, Model | ✅ |
| `ResponseErrorEntry.cs` | Framework | Simple POCO: `Message`, `Property` for error details | ✅ |
| `ValidationResultExtensions.cs` | Framework | Extension: `ToResponseErrors()` converts FluentValidation results | ✅ |
| `StringExtensions.cs` | Framework | Single utility: `ToLowerFirstCharacter()` for camelCase conversion | ✅ |
| `ILogRecorder.cs` | Framework | Abstraction with Info/Warn/Error/Critical/Debug/Trace methods | ✅ |
| `LogRecorder.cs` | Framework | Wraps `ILogger<T>` from Microsoft.Extensions.Logging | ✅ |
| `SettingsProvider.cs` | **Gray Area** | Pattern is generic. Contains `SecuritySettings`, `JwtSettings`, `DatabaseSettings`, `RateLimitingSettings`, `RequestSizeLimitSettings` - all fairly universal | ✅ |
| `ApplicationWarmup.cs` | **App-Specific** | Directly references `PagePlay.Site.Application.Accounts.Domain.Models.User` | ✅ |

**Decision D1 (SettingsProvider):** The settings schema (Security, Database, RateLimiting, RequestSizeLimits) is actually quite universal. Options:
1. Move entire SettingsProvider + all settings classes to framework (most apps need these)
2. Framework provides interface only, app defines schema
3. Framework provides base settings (Database, RateLimiting), app extends for domain-specific (unlikely to need custom security schema)

**Recommendation:** Option 1 - move to framework. These are standard web app settings.

---

### Infrastructure/Core/Domain

| File | Classification | Notes | Reviewed |
|------|----------------|-------|----------|
| `IEntity.cs` | Framework | Interface: `Id (long)`, `CreatedAt`, `UpdatedAt` | ✅ |
| `INormalizeValues.cs` | Framework | Interface with single `Normalize()` method for pre-save value cleanup | ✅ |

**Review Complete:** Both are generic and belong in framework.

---

### Infrastructure/Data

| File | Classification | Notes | Reviewed |
|------|----------------|-------|----------|
| `Repositories/IRepository.cs` | Framework | Generic interface with `Get`, `GetForUpdate`, `List`, `Exists`, `Add`, `Update`, `Delete`, `SaveChanges`, `BeginTransactionScope`, `Health`. Also defines `ITransactionScope`. | ✅ |
| `Repositories/Repository.cs` | **Gray Area** | Implementation has hard dependency on `AppDbContext`. For framework: needs to become generic `Repository<TDbContext>` or abstract base. | ✅ |
| `Specifications/ISpecification.cs` | Framework | Interface: `Criteria` (expression), `Includes` (list of includes) | ✅ |
| `Specifications/Specification.cs` | Framework | Implementation with fluent `Include()` method | ✅ |
| `ICacheProvider.cs` | **Delete** | Empty placeholder class (not even an interface!). Remove or implement properly. | ✅ |
| `AppDbContext.cs` | App-Specific | Auto-discovers IEntity types from assembly. Has SaveChangesAsync override for timestamps. | ✅ |
| `AppDbContextFactory.cs` | App-Specific | `IDesignTimeDbContextFactory<AppDbContext>` for EF migrations CLI | ✅ |
| `Migrations/*.cs` (8 files) | App-Specific | PagePlay database schema migrations | ✅ |

**Key Finding - Repository Coupling:**
The `Repository.cs` implementation has `IDbContextFactory<AppDbContext>` injected directly. For framework extraction, two options:

1. **Make Repository generic:** `Repository<TContext> where TContext : DbContext`
   - Pro: Clean, type-safe
   - Con: Every app must register with their context type

2. **Use non-generic DbContext:** Have DI provide `IDbContextFactory<DbContext>` at runtime
   - Pro: Simpler registration
   - Con: Loses compile-time type safety

**Recommendation:** Option 1 - Make Repository generic. Apps register as:
```csharp
services.AddScoped<IRepository, Repository<AppDbContext>>();
```

**Decision D3 (DbContext):** Should framework provide a `FrameworkDbContext` base class with:
- Auto-discovery of IEntity types
- SaveChangesAsync with timestamp handling
- Other common behaviors

**Recommendation:** Yes - provide abstract base. Apps inherit:
```csharp
public class AppDbContext : FrameworkDbContext { }
```

---

### Infrastructure/Security

| File | Classification | Notes | Reviewed |
|------|----------------|-------|----------|
| `PasswordHasher.cs` | Framework | Interface + impl. BCrypt with HMAC-SHA256 pepper. Depends on `ISettingsProvider.Security.PasswordPepper`. | ✅ |
| `JwtTokenService.cs` | Framework | Interface + impl. JWT generation/validation. Depends on `ISettingsProvider.Security.Jwt.*`. Includes `TokenClaims` class. | ✅ |
| `LoggedInAuthContext.cs` | Framework | Contains `ICurrentUserContext` (interface) and `CurrentUserContext` (impl). Simple: `UserId: long?`. | ✅ |
| `PopulateAuthContextFilter.cs` | Framework | `IEndpointFilter` that populates `CurrentUserContext` from JWT. Returns 401 if not authenticated. | ✅ |
| `UserIdentityService.cs` | Framework | Interface + impl. Extracts user ID from `ClaimsPrincipal` (supports both `ClaimTypes.NameIdentifier` and `JwtRegisteredClaimNames.Sub`). | ✅ |

**Key Finding - All Security is Generic:**
All security code is actually generic and could be framework! The perceived "app-specificity" was from the coupling to `ISettingsProvider`. Since the settings schema (Security, Jwt) is also generic (Decision D1), all of Security can move to framework.

**Decision D2 (Auth) - Resolved:**
Framework provides complete auth infrastructure:
- `IPasswordHasher` + `PasswordHasher` (BCrypt)
- `IJwtTokenService` + `JwtTokenService`
- `ICurrentUserContext` + `CurrentUserContext`
- `IUserIdentityService` + `UserIdentityService`
- `PopulateAuthContextFilter`

Apps use these as-is. No extension points needed since:
- User ID is always `long` (per `IEntity`)
- JWT claims are standard
- Password hashing algorithm is correct (BCrypt + pepper)

---

### Infrastructure/Web

| File | Classification | Notes | Reviewed |
|------|----------------|-------|----------|
| **Components/** | | | |
| `IView.cs` | Framework | `IView` interface, `DataDependencies` class (with `From<T>()` using reflection for DomainName), `IDataContext` + `DataContext` impl | ✅ |
| `ViewContext.cs` | Framework | Parses `X-Component-Context` JSON header from client | ✅ |
| `ViewFactory.cs` | **Gray Area** | Uses `typeof(IView).Assembly` to discover views - needs assembly configuration for framework | ✅ |
| **Data/** | | | |
| `IDataProvider.cs` | Framework | `IDataProvider` marker + `IDataProvider<TContext>` with `FetchTyped(userId)` | ✅ |
| `DataLoader.cs` | Framework | Fluent API entry point, `IDataLoader` + impl. Includes `DataLoadException`. | ✅ |
| `DataLoaderBuilder.cs` | Framework | Builder pattern impl for `With<T>().Load()` | ✅ |
| **Framework/** | | | |
| `FrameworkOrchestrator.cs` | Framework | `IFrameworkOrchestrator` + impl. Orchestrates data loading, view rendering, OOB updates. Uses reflection for fluent chain building. | ✅ |
| **Html/** | | | |
| `HtmlFragment.cs` | Framework | Static helpers: `WithId()`, `WithOob()`, `InjectOob()` for HTMX OOB responses | ✅ |
| `HtmlHelpers.cs` | Framework | HTML encoding/generation utilities | ✅ |
| `HtmxForm.cs` | Framework | HTMX form builder helpers | ✅ |
| **Http/** | | | |
| `AntiforgeryTokenProvider.cs` | Framework | CSRF token provider wrapping ASP.NET antiforgery | ✅ |
| `CookieManager.cs` | Framework | Cookie read/write abstraction | ✅ |
| **Middleware/** | | | |
| `RequestLoggingMiddleware.cs` | Framework | Logs requests with duration, user, correlation ID. Uses `ILogRecorder`. | ✅ |
| `RequestSizeLimitMiddleware.cs` | Framework | Enforces request size limits from `ISettingsProvider` | ✅ |
| `ResponseCompressionMiddleware.cs` | Framework | Configures gzip/brotli compression | ✅ |
| `SecurityHeadersMiddleware.cs` | Framework | Adds security headers (CSP, X-Frame-Options, etc.) | ✅ |
| `RateLimitingMiddleware.cs` | Framework | Rate limiting per user/IP from `ISettingsProvider` | ✅ |
| **Mutations/** | | | |
| `DataMutations.cs` | Framework | Simple class with `Domains` list and `For(params string[])` factory | ✅ |
| **Pages/** | | | |
| `PageInteractionBase.cs` | Framework | Abstract base for interactions. Handles routing, auth, performer execution, OOB. Has `BuildOobResult()`, `BuildOobOnly()`, `BuildOobResultWith()` helpers. | ✅ |
| `PageInteraction.cs` | Framework | Route helper: `GetRoute(routeBase, action)` | ✅ |
| **Routing/** | | | |
| `IEndpoint.cs` | Framework | Simple interface: `Map(IEndpointRouteBuilder)` | ✅ |
| `IEndpointRoutes.cs` | Framework | Interface: `MapRoutes(IEndpointRouteBuilder)` | ✅ |
| `EndpointRoutesExtensions.cs` | **Gray Area** | Uses `Assembly.GetExecutingAssembly()` - needs assembly configuration for framework | ✅ |
| `RouteRespond.cs` | Framework | Response helpers for endpoints | ✅ |

**Key Finding - Assembly Binding Issues:**
Two files use assembly reflection that would break in framework:

1. **ViewFactory.cs** - `typeof(IView).Assembly.GetTypes()` discovers views from current assembly
2. **EndpointRoutesExtensions.cs** - `Assembly.GetExecutingAssembly().GetTypes()` discovers endpoint routes

**Solution for Framework:**
Framework provides configurable discovery:
```csharp
// Option 1: Assembly parameter
services.AddArchTypeFramework(typeof(Program).Assembly);

// Option 2: Explicit registration
services.AddArchTypeFramework(options => {
    options.ScanAssembly(typeof(Program).Assembly);
    options.ScanAssembly(typeof(SharedViews).Assembly);
});
```

**Review Complete:** All Web code is framework, with assembly configuration needed for ViewFactory and EndpointRoutesExtensions.

---

### Infrastructure/UI

| File | Classification | Notes | Reviewed |
|------|----------------|-------|----------|
| `IElement.cs` | Framework | Base `IElement` interface + slot markers (`IHeaderContent`, `IBodyContent`, `IFooterContent`, `IFieldContent`) + `ElementBase` record with collection initializer support | ✅ |
| **Vocabulary/** | | | |
| `Button.cs` | Framework | `Button` record with `Importance` enum, HTMX support (`Action`, `Target`, `Swap`, `ModelId`), fluent builder. Also `SwapStrategy`, `ButtonType` enums. | ✅ |
| `Card.cs` | Framework | `Card` record with slot-based API: `Header()`, `Body()`, `Footer()` | ✅ |
| `FeedbackElements.cs` | Framework | `Alert` (with `AlertTone` enum), `EmptyState` (with `EmptyStateSize` enum) | ✅ |
| `FormElements.cs` | Framework | `Input` (with `InputType` enum), `Label`, `Field` (with error support), `Form` (HTMX), `Checkbox` (HTMX toggle) | ✅ |
| `Layout.cs` | Framework | `Stack`, `Row`, `Grid` with `For` (purpose) and `Columns` enums | ✅ |
| `ListElements.cs` | Framework | `List` (with `ListStyle` enum), `ListItem` (with `ListItemState` enum) | ✅ |
| `PageStructure.cs` | Framework | `Page`, `Section`, `PageTitle`, `SectionTitle` with fluent `Id()` method | ✅ |
| `Slots.cs` | Framework | `Header`, `Body`, `Footer` extending `ElementBase` | ✅ |
| `Text.cs` | Framework | Simple `Text` record with `Content` property | ✅ |
| **Rendering/** | | | |
| `HtmlRenderer.cs` | Framework | `IHtmlRenderer` + impl. Big switch statement renders each element type to semantic HTML classes. ~530 lines. | ✅ |
| `ThemeCompiler.cs` | Framework | Compiles YAML theme to CSS. Generates CSS layers (tokens, base, components). Uses YamlDotNet. | ✅ |
| `ThemeCompilerCli.cs` | Framework | CLI entry point for theme compilation tooling | ✅ |
| **Themes/** | | | |
| `default.theme.yaml` | App-Specific | PagePlay's theme file - token values, component mappings | ✅ |

**Review Complete:** All UI code is framework. The only app-specific item is the theme YAML file itself.

**Decision D4 (Theme Files) - Resolved:**
- Framework provides ThemeCompiler and default token/component structure
- Apps provide their own theme.yaml files
- Framework could optionally provide a "starter" theme that apps can copy/customize

---

### Infrastructure/Dependencies

| File | Classification | Notes | Reviewed |
|------|----------------|-------|----------|
| `DependencyResolver.cs` | App-Specific | PagePlay-specific DI: binds performers, providers, views, pages, interactions. Contains explicit app references. | ✅ |
| `ServiceCollectionExtensions.cs` | Framework | Generic `Replace<TInterface, TImpl>()` extension for swapping DI registrations | ✅ |

**Review Complete:** DependencyResolver is app-specific. ServiceCollectionExtensions is framework.

**Framework Registration Pattern:**
Framework should provide `AddArchTypeFramework()` extension that apps call:
```csharp
// In Program.cs
services.AddArchTypeFramework(options => {
    options.ScanAssembly(typeof(Program).Assembly);
});

// Then app-specific registrations
DependencyResolver.Bind(services);
```

---

## Decisions to Make

### D1: Settings Provider Pattern
**Question:** How should configuration be handled?
**Options:**
1. Framework provides `ISettingsProvider` interface only, app implements
2. Framework provides generic settings loading, app defines schema
3. Keep current approach but move base to framework

**Decision:** ⬜ TBD

---

### D2: Auth Abstraction
**Question:** Should framework provide auth infrastructure?
**Options:**
1. Framework provides `ICurrentUserContext` interface, app implements auth
2. Framework provides JWT infrastructure with configurable options
3. Leave auth entirely to application

**Decision:** ⬜ TBD

---

### D3: DbContext Pattern
**Question:** Should framework provide a base DbContext?
**Options:**
1. Framework provides `FrameworkDbContext` base with timestamps, entity discovery
2. App owns DbContext entirely, framework just needs `DbContext` for Repository<T>
3. Framework provides abstract `DbContextBase`, app inherits

**Decision:** ⬜ TBD

---

### D4: Theme File Location
**Question:** Where do theme files belong?
**Options:**
1. Theme files are always app-specific (stay in app)
2. Framework provides default theme, app can override
3. Framework provides theme structure, app provides values

**Decision:** ⬜ TBD

---

## Implementation Phases

### Phase 1: Framework Project Setup
- [ ] Set up ArchType.Framework project structure
- [ ] Add required NuGet dependencies
- [ ] Establish namespace conventions

### Phase 2: Extract Core
- [ ] Move Core/Domain to framework
- [ ] Move Core/Application (framework parts) to framework
- [ ] Update namespaces
- [ ] Ensure PagePlay compiles

### Phase 3: Extract Data Infrastructure
- [ ] Move Repository pattern to framework
- [ ] Move Specification pattern to framework
- [ ] Keep AppDbContext in app
- [ ] Ensure PagePlay compiles

### Phase 4: Extract Web Infrastructure
- [ ] Move Components to framework
- [ ] Move Data (providers, loaders) to framework
- [ ] Move Framework (orchestrator) to framework
- [ ] Move Html helpers to framework
- [ ] Move Http utilities to framework
- [ ] Move Middleware to framework
- [ ] Move Mutations to framework
- [ ] Move Pages (base classes) to framework
- [ ] Move Routing to framework
- [ ] Ensure PagePlay compiles

### Phase 5: Extract UI Infrastructure
- [ ] Move IElement and Vocabulary to framework
- [ ] Move Rendering (HtmlRenderer, ThemeCompiler) to framework
- [ ] Keep theme file in app
- [ ] Ensure PagePlay compiles

### Phase 6: Extract Security Infrastructure
- [ ] Move PasswordHasher to framework
- [ ] Resolve auth abstraction decisions
- [ ] Ensure PagePlay compiles

### Phase 7: Finalize
- [ ] Clean up Dependencies folder
- [ ] Update documentation
- [ ] Verify all tests pass
- [ ] Remove dead code from PagePlay.Site

---

## Session Log

### Session 1 (2025-12-12) - Deep Review Complete

**Completed:**
- Created this tracking document
- Deep reviewed all 72 Infrastructure files (read actual code)
- Final classification: 56 Framework, 12 App-Specific, 4 Gray Area, 1 Delete
- Resolved all 4 decisions:
  - D1 (SettingsProvider): Move to framework - settings schema is universal
  - D2 (Auth): Move all to framework - complete auth infrastructure is generic
  - D3 (DbContext): Provide `FrameworkDbContext` base class with timestamp handling
  - D4 (Theme): Framework provides compiler, apps provide theme files

**Key Findings:**

1. **Security is 100% framework** - All auth code (JWT, password hashing, user context) is generic
2. **SettingsProvider schema is universal** - Security, Database, RateLimiting, RequestSizeLimits work for any app
3. **Repository needs generics** - Current impl hardcodes `AppDbContext`, needs `Repository<TContext>`
4. **Assembly discovery needs config** - ViewFactory and EndpointRoutesExtensions use hardcoded assembly scanning
5. **ICacheProvider is dead code** - Empty placeholder class, should be deleted

**Gray Areas Resolved:**
- `SettingsProvider.cs` → Framework (universal settings schema)
- `Repository.cs` → Framework (after making generic)
- `ViewFactory.cs` → Framework (after adding assembly config)
- `EndpointRoutesExtensions.cs` → Framework (after adding assembly config)

**App-Specific Items (Final):**
1. `ApplicationWarmup.cs` - References User entity directly
2. `AppDbContext.cs` - Inherits FrameworkDbContext
3. `AppDbContextFactory.cs` - Design-time factory
4. `Migrations/` - 8 migration files
5. `DependencyResolver.cs` - App DI registrations
6. `default.theme.yaml` - App theme

**Next Session:**
- Phase 1: Set up ArchType.Framework project structure
- Phase 2: Start extracting Core (domain, application basics)
- Consider: Should framework provide `AddArchTypeFramework()` registration helper?

---

## Notes & Context

### Why "ArchType"?
The framework name reflects its purpose: providing the archetypal patterns for building web applications with this architecture. It's the "type" that defines the "arch"itecture.

### Namespace Convention (Proposed)
```
ArchType.Framework.Core.Application
ArchType.Framework.Core.Domain
ArchType.Framework.Data
ArchType.Framework.Security
ArchType.Framework.Web
ArchType.Framework.UI
```

### File Location After Extraction
```
ArchType.Framework/
├── Core/
│   ├── Application/    # IPerformer, ApplicationResult, etc.
│   └── Domain/         # IEntity, INormalizeValues
├── Data/
│   ├── Repositories/   # IRepository, Repository
│   └── Specifications/ # ISpecification, Specification
├── Security/           # IPasswordHasher, PasswordHasher
├── Web/
│   ├── Components/     # IView, ViewFactory, etc.
│   ├── Data/           # IDataProvider, DataLoader
│   ├── Framework/      # FrameworkOrchestrator
│   ├── Html/           # HtmlFragment, HtmlHelpers
│   ├── Http/           # AntiforgeryTokenProvider, CookieManager
│   ├── Middleware/     # All middleware
│   ├── Mutations/      # DataMutations
│   ├── Pages/          # PageInteractionBase
│   └── Routing/        # IEndpoint, extensions
└── UI/
    ├── IElement.cs
    ├── Vocabulary/     # All elements
    └── Rendering/      # HtmlRenderer, ThemeCompiler
```

---

**Last Updated:** 2025-12-12
**Document Version:** 1.0
