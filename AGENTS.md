# Maliev.LifecycleService Coding Agents Guide

This document outlines the architectural standards, code style, and operational procedures for AI agents working on the Maliev.LifecycleService repository.

## Project Architecture

The solution follows **Clean Architecture** principles:

- **Domain** (`Maliev.LifecycleService.Domain`)
  - Contains core entities, enums, exceptions, and value objects.
  - **No dependencies** on other projects.
  - Entities should be rich where possible, but currently appear anemic with public setters in some contexts.

- **Application** (`Maliev.LifecycleService.Application`)
  - Contains business logic, CQRS patterns (MediatR), Validators, DTOs, and Interfaces.
  - Depends on **Domain**.
  - Defines interfaces for infrastructure (e.g., `IOnboardingRepository`, `IEventPublisher`).

- **Infrastructure** (`Maliev.LifecycleService.Infrastructure`)
  - Implements interfaces defined in Application.
  - Handles Database (EF Core), Messaging (MassTransit), and external integration.
  - Depends on **Application** and **Domain**.

- **Api** (`Maliev.LifecycleService.Api`)
  - Entry point (ASP.NET Core Web API).
  - Contains Controllers, Middleware, and Program.cs configuration.
  - Depends on **Application** and **Infrastructure**.

- **Tests** (`Maliev.LifecycleService.Tests`)
  - Unit and Integration tests using **xUnit**.

## Build & Test Commands

Use the following commands from the root directory (`B:\maliev\Maliev.LifecycleService`):

### Build
```bash
dotnet build Maliev.LifecycleService.slnx
```

### Test
Run all tests:
```bash
dotnet test
```

Run a single test (filtering by FullyQualifiedName):
```bash
dotnet test --filter "FullyQualifiedName~Namespace.ClassName.MethodName"
```
*Example:*
```bash
dotnet test --filter "FullyQualifiedName~Maliev.LifecycleService.Tests.Unit.Validators.StartOnboardingValidatorTests.Validate_ValidInput_ReturnsNoErrors"
```

## Code Style & Conventions

### General
- **Framework**: .NET 10.0 (ASP.NET Core).
- **Namespaces**: Use **file-scoped namespaces** (e.g., `namespace Maliev.LifecycleService.Domain;`).
- **Usings**: Implicit usings are **enabled**. Do not add `System`, `System.Linq` etc. unless necessary.
- **Async/Await**: Use `async` for all I/O bound operations. Always pass `CancellationToken` to async methods where supported.

### Naming
- **Classes/Interfaces/Enums**: `PascalCase`.
- **Methods/Properties**: `PascalCase`.
- **Private Fields**: `_camelCase` (e.g., `_context`, `_logger`).
- **Parameters/Locals**: `camelCase`.
- **Interfaces**: Prefix with `I` (e.g., `ITemplateRepository`).
- **Test Methods**: `MethodName_StateUnderTest_ExpectedBehavior`.

### Formatting
- **Indentation**: 4 spaces.
- **Braces**: Allman style (opening brace on a new line).
- **Documentation**: Use XML documentation (`///`) for public members in Domain, Application, and Infrastructure layers.

### Implementation Patterns

#### CQRS (MediatR)
- **Commands**: Immutable records implementing `IRequest` or `IRequest<T>`.
  ```csharp
  public record CreateItemCommand(string Name, Guid UserId) : IRequest<Guid>;
  ```
- **Handlers**: Classes implementing `IRequestHandler<TCommand, TResult>`.
  ```csharp
  public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, Guid>
  ```

#### Data Access
- **Repositories**: Return `Task<T?>` for single item lookups.
- **EF Core**: Use `LifecycleDbContext`.
- **Async**: Always use `Async` versions of EF Core methods (e.g., `FirstOrDefaultAsync`, `ToListAsync`).

#### Error Handling
- Use custom exceptions defined in `Domain.Exceptions` for business logic errors.
- Validate inputs early using Data Annotations or Guard clauses.
- Controller actions should return standard HTTP responses (200, 201, 400, 404).

#### Testing
- **Framework**: xUnit.
- **Mocking**: Use **Moq** for unit tests.
- **Integration Tests**: Use `WebApplicationFactory` and Testcontainers (Postgres, RabbitMQ, Redis).
- **Structure**: Use `// Arrange`, `// Act`, `// Assert` comments in test bodies.

### Testing Strategy (4-Tier Pyramid Context)

This service's tests cover **Tier 1 (Unit)** and **Tier 2 (Service Integration)** of the Maliev testing pyramid:

| Tier | What to Test | Infrastructure |
|------|-------------|---------------|
| **Unit** | Business logic, domain models, service methods with mocked dependencies | None (mocks only) |
| **Service Integration** | API endpoints, database persistence, permission enforcement, input validation | `BaseIntegrationTestFactory` + Testcontainers (Postgres/Redis/RabbitMQ) |

**Tier 3 (System Integration)** — cross-service workflows and event chains — is tested in `Maliev.Aspire.Tests/`.

#### Key Rules
- Use `BaseIntegrationTestFactory<TProgram, TDbContext>` for integration tests (real Testcontainers, never InMemoryDatabase)
- Test naming: `MethodName_StateUnderTest_ExpectedBehavior`
- Minimum 80% code coverage
- Use `[Fact]` for single cases, `[Theory]` for parameterized tests

> Full ecosystem test strategy: `Maliev.Aspire.Tests/TEST_PLAN.md`

## Dependencies & Tools
- **ORM**: Entity Framework Core (Npgsql).
- **Messaging**: MassTransit (RabbitMQ).
- **Caching**: StackExchange.Redis.
- **Testing**: xUnit, Moq, Testcontainers.

## AI Rules (Implicit)
- **Modifications**: When modifying code, preserve existing style and conventions.
- **Safety**: Do not commit secrets.
- **Verification**: Always compile and run relevant tests before confirming a task.

Instructions from: C:\Users\natth\.claude\CLAUDE.md
Always verify any changes you made with successful build. Never assume any changes you made will not result in broken build.


## Git & Version Control — Mandatory Rules

### 🚨 CRITICAL: Always Commit Code Changes (Non-Negotiable)
- **You MUST commit your changes to the local repository after completing any meaningful unit of work.**
- **Never accumulate uncommitted changes.** Do not wait until end of session or until something breaks.
- **Commit early and often** — if a change is meaningful (even a small fix or refactor), commit it.
- **You do NOT need to push to remote** — local commits are sufficient to protect against accidental loss.
- **If you are unsure whether to commit, commit anyway.** Extra commits are harmless; lost work is irreversible.
- This rule applies even if you are just "testing" or "exploring" — use git branches to isolate experimental work and commit those changes too.

### 🚨 CRITICAL: Never Use `git checkout` to Restore Broken Files
- **NEVER use `git checkout` to restore or recover files.** This operation discards uncommitted changes permanently and will result in data loss.
- **To undo/recover from broken files: first commit your current changes, then use `git revert` or `git reset --soft` to safely undo.**

## Database & EF Core — Mandatory Rules

### EF Core Design Package
- ❌ `Microsoft.EntityFrameworkCore.Design` MUST NOT be in Api projects
- ✅ It belongs ONLY in the Infrastructure (or Data) project where migrations live
- Migration commands must target Infrastructure as both project and startup-project (since EF Core Design package is in Infrastructure):
  ```
  dotnet ef migrations add <Name> --project Maliev.<Domain>Service.Infrastructure --startup-project Maliev.<Domain>Service.Infrastructure
  ```

### PostgreSQL xmin Concurrency — Mandatory Pattern
Use shadow property ONLY. Never add a Xmin/xmin property to domain entities.
```csharp
entity.Property<uint>("xmin").HasColumnType("xid").IsRowVersion();
```
- ❌ Never use `UseXminAsConcurrencyToken()` (removed in Npgsql EF v7)
- ❌ Never use entity property `public uint Xmin { get; set; }` or `public uint xmin { get; set; }`
- ❌ Never use `.Ignore(e => e.Xmin)` — remove the entity property instead
