# Infrastructure/UI - Context Loading Guide

When starting a new session or resuming work on the Closed-World UI system, read these files in order:

## Foundation - Design Experiments (Read First)
These documents explain the WHY behind the Closed-World UI architecture:

1. `../../../project-docs/experiments/styles/A-01_css-architecture-evolution-timeline.md`
2. `../../../project-docs/experiments/styles/A-02_css-architecture-by-org-structure.md`
3. `../../../project-docs/experiments/styles/A-03_native-css.md`
4. `../../../project-docs/experiments/styles/A-04_closed-world-ui-methodology.md`
5. `../../../project-docs/experiments/styles/B-00_README.md`
6. `../../../project-docs/experiments/styles/B-01_philosophy.md`
7. `../../../project-docs/experiments/styles/B-02_vocabulary.md`
8. `../../../project-docs/experiments/styles/B-03_type-system.md`
9. `../../../project-docs/experiments/styles/B-04_theme-authoring.md`
10. `../../../project-docs/experiments/styles/B-05_rendering.md`

## Core Implementation (Read Second)
11. `IComponent.cs` - Base component interfaces and ComponentBase
12. `Themes/default.theme.yaml` - Theme definition (design tokens and component styling)

## Vocabulary (Component Types)
13. `Vocabulary/Slots.cs` - Header/Body/Footer slot containers
14. `Vocabulary/Card.cs` - Card container with slots
15. `Vocabulary/Layout.cs` - Stack/Row/Grid layout primitives with semantic spacing
16. `Vocabulary/PageStructure.cs` - Page/Section/Titles
17. `Vocabulary/Text.cs` - Text content component
18. `Vocabulary/Button.cs` - Button with Importance levels

## Rendering System
19. `Rendering/HtmlRenderer.cs` - Semantic components → HTML with CSS classes
20. `Rendering/ThemeCompiler.cs` - YAML theme → CSS generation
21. `Rendering/ThemeCompilerCli.cs` - CLI entry point for build integration

## Philosophy
- **Closed-World**: Constrained vocabulary, no escape hatches (no className, no inline styles)
- **Semantic declarations**: Developers declare WHAT (purpose), theme controls HOW (appearance)
- **Type safety**: Slot interfaces enforce valid component composition
- **Theme-driven**: All styling controlled via YAML → CSS compilation

## Build Integration
- MSBuild auto-compiles theme on build (see `PagePlay.Site.csproj` CompileTheme target)
- Incremental: Only recompiles when YAML changes
- Output: `wwwroot/css/closed-world.css` (auto-generated, DO NOT EDIT)

## Example Usage
See: `/Pages/StyleTest/StyleTest.Page.htmx.cs` for reference implementation
