# Brief: Registration Page

**Status:** Complete
**Created:** 2026-01-10

---

## Goal

Create a registration page UI that allows users to create accounts using the existing Register performer.

## Problem

The backend registration logic exists (`Register.Performer`) but there's no UI - users cannot register through the application.

## Success Signal

- [x] User can fill out registration form (email, password, confirm password)
- [x] Validation errors display correctly (empty fields, invalid email, password mismatch, too short)
- [x] Duplicate email error displays correctly
- [x] Successful registration redirects to login page

**Verified:** 2026-01-10 - All signals confirmed via manual testing

## Constraints

- Must use existing `Register.Performer` (no backend changes)
- Must follow existing page patterns (Login page as reference)
- Must use Closed-World UI vocabulary (Flowbite-styled elements)

---

## Phases

| # | Goal | Status |
|---|------|--------|
| 1 | Create Registration Page view (form, notifications) | ✅ |
| 2 | Create Registration Interaction & Route (wire up performer) | ✅ |

---

## Phase 1: Registration Page View

**Goal:** Create the registration page view with form and notification rendering methods

### Research
- [x] Review existing Register.BoundaryContracts.cs for field requirements
- [x] Review Login.Page.cs as reference pattern

### Findings

**Fields required (from RegisterRequest):**
- Email (required, valid format)
- Password (required, min 8 chars)
- ConfirmPassword (required, must match password)

**Pattern from Login.Page.cs:**
- Interface `IRegisterPageView : IView`
- `ViewId` property
- `DataDependencies.None` (static page)
- Section with PageTitle, notifications section, form
- `RenderRegisterForm()` method
- `RenderErrorNotification(string)` method
- `RenderSuccessNotification(string)` method (may not need)

### Adjustments
None - straightforward implementation following existing pattern.

### TDD Plan

1. **Test:** RegisterPage renders form with email field
   **Impl:** Create Register.Page.cs with basic structure and email field

2. **Test:** RegisterPage renders form with password and confirmPassword fields
   **Impl:** Add password fields to form

3. **Test:** RegisterPage.RenderErrorNotification returns alert HTML
   **Impl:** Add notification render methods

### Done
- Created `Pages/Register/Register.Page.cs`
- `IRegisterPageView` interface with render methods
- Form with email, password, confirmPassword fields
- Action points to `/interaction/register/create`
- Auto-discovered by DI (no manual registration needed)

---

## Phase 2: Registration Interaction & Route

**Goal:** Create the interaction and route to wire up the registration form to the performer

### Research
- [x] Review Login.Route.cs for endpoint pattern
- [x] Review Authenticate.Interaction.cs for interaction pattern

### Findings

**Route pattern (from Login.Route.cs):**
- Interface `IRegisterPageInteraction : IEndpoint`
- `RegisterPageEndpoints` class with `PAGE_ROUTE = "register"`
- MapGet for page render
- Loop to map interactions

**Interaction pattern (from Authenticate.Interaction.cs):**
- Extends `PageInteractionBase<RegisterRequest, RegisterResponse, IRegisterPageView>`
- `RouteBase` = page route
- `RouteAction` = "create" (or similar)
- `RequireAuth` = false
- `OnSuccess`: redirect to login page
- `RenderError`: show error notification OOB

### Adjustments
None - straightforward implementation following existing pattern.

### TDD Plan

1. **Test:** CreateAccount.Interaction calls performer and redirects on success
   **Impl:** Create Interactions/CreateAccount.Interaction.cs

2. **Test:** CreateAccount.Interaction renders error on failure
   **Impl:** Implement RenderError method

3. **Test:** RegisterPageEndpoints maps GET /register route
   **Impl:** Create Register.Route.cs

4. **Integration:** Wire up DI registration in DependencyResolver.cs
   **Impl:** Register view, endpoints, interaction

### Done
- Created `Pages/Register/Interactions/CreateAccount.Interaction.cs`
  - Extends `PageInteractionBase<RegisterRequest, RegisterResponse, IRegisterPageView>`
  - On success: redirects to `/login`
  - On error: renders error notification OOB
- Created `Pages/Register/Register.Route.cs`
  - `IRegisterPageInteraction` marker interface
  - `RegisterPageEndpoints` maps GET /register and loops interactions
- Updated `DependencyResolver.cs`
  - Added `RegisterPageEndpoints` to bindPageEndpoints
  - Added `CreateAccountInteraction` to bindPageInteractions
- Build verified successful

---

## Open Questions

None currently - requirements are clear.

---

## Session Log

### 2026-01-10 - Session 1
- Created brief from conversation
- Reviewed existing performer (3 fields: email, password, confirmPassword)
- Reviewed Login page pattern as reference
- Defined 2 phases: View, then Interaction/Route

### 2026-01-10 - Session 2
- Completed Phase 1: Registration Page View
- Created `Pages/Register/Register.Page.cs` following Login page pattern
- Build verified successful

### 2026-01-10 - Session 3
- Completed Phase 2: Registration Interaction & Route
- Created `CreateAccount.Interaction.cs` following Authenticate.Interaction.cs pattern
- Created `Register.Route.cs` following Login.Route.cs pattern
- Updated DependencyResolver.cs with registrations
- Build verified successful
- Brief complete
