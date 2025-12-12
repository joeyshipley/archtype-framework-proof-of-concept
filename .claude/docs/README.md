# Project Philosophy & Architecture

**Version:** 2.0
**Last Updated:** 2025-12-12
**Status:** Foundation Document

---

## On Consistency and Team Velocity

This project prioritizes consistency and predictability over individual optimization. Architectural decisions are made to ensure that any developer—whether junior, senior, or AI agent—can immediately understand where code belongs and how to extend it. When everyone follows the same patterns, the entire team moves faster: code reviews are quicker, onboarding is smoother, and debugging is easier because you know what to expect.

**Framework handles infrastructure complexity.** Developers focus on business features, not plumbing. The framework provides test bases, DI registration, repository patterns, and migration discovery—all the boilerplate that distracts from feature work. This serves two audiences: feature developers who shouldn't think about infrastructure, and framework developers who ensure patterns remain consistent and self-enforcing.

We've designed these patterns to be self-enforcing through code rather than relying on documentation. For example, migrations automatically discover their location through the DbContextFactory, repositories follow a consistent generic base pattern with specifications, DI registration is centralized in DependencyResolver, and the vertical slice architecture makes it obvious where new features belong. The constraints aren't limitations on what you can build, but guardrails that prevent individuals from breaking architectural consistency with clever solutions. If you find yourself fighting the established patterns, it's worth asking whether the pattern needs to evolve for everyone, or if there's a way to solve your problem within the existing structure. The goal is a codebase where the "right way" is also the "easy way," so the architecture serves the team rather than individual preferences.

## Framework Purpose

This documentation describes the technical patterns and architectural decisions that make the above principles concrete. Understanding these patterns helps you work effectively within the codebase and make choices that align with the project's goals. The framework exists to serve both as a real application and as a reference implementation that demonstrates how constrained architecture enables developer productivity.

---

## Additional Documentation

- **[Philosophy](./README.PHILOSOPHY.md)** - Core beliefs, HTTP-first rationale, consistent complexity, and patterns
- **[Architecture Reference](./README.ARCHITECTURE_REFERENCE.md)** - Interfaces, data flow, and implementation details
- **[Workflow](./workflow/)** - Development modes and processes
