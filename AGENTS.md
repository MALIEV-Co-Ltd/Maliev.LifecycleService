# Maliev.LifecycleService Coding Agents Guide

This document outlines the architectural standards, code style, and operational procedures for AI agents working on the Maliev.LifecycleService repository.

> **Workspace root** `B:\maliev` contains **41 independent git repos**. Each `Maliev.*` folder and `maliev-gitops` is its own repo. There is no single repo at the workspace root. Always work within the target service directory.

---

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

### Workspace Structure
```
Maliev.LifecycleService/
├── Maliev.LifecycleService.Api/           # Controllers, Consumers, Middleware
├── Maliev.LifecycleService.Application/   # Use cases, DTOs, Interfaces, Handlers
├── Maliev.LifecycleService.Domain/        # Entities, value objects, domain interfaces
├── Maliev.LifecycleService.Infrastructure/ # EF Core DbContext, repositories, HTTP clients
├── Maliev.LifecycleService.Tests/         # Unit + Integration tests (xUnit)
├── Directory.Build.props                  # Central package versioning
└── Maliev.LifecycleService.slnx          # Solution file (.slnx preferred over .sln)
```

---

## Build, Test & Lint Commands

All commands run from within this service directory (`B:\maliev\Maliev.LifecycleService`).

```powershell
# Build (treats warnings as errors — all must be fixed)
dotnet build Maliev.LifecycleService.slnx

# Run all tests
dotnet test Maliev.LifecycleService.slnx --verbosity normal

# Run a single test method
dotnet test --filter "FullyQualifiedName~Maliev.LifecycleService.Tests.Unit.Validators.StartOnboardingValidatorTests.Validate_ValidInput_ReturnsNoErrors"

# Run all tests in a class
dotnet test --filter "FullyQualifiedName~StartOnboardingValidatorTests"

# Run with code coverage
dotnet test Maliev.LifecycleService.slnx --collect:"XPlat Code Coverage"

# Format check
dotnet format Maliev.LifecycleService.slnx

# EF Core migrations (Infrastructure project only)
dotnet ef migrations add <Name> --project Maliev.LifecycleService.Infrastructure --startup-project Maliev.LifecycleService.Infrastructure
```

---

## Code Style & Conventions

### C# Naming & Formatting
- **Namespaces**: File-scoped (`namespace Maliev.LifecycleService.Domain.Entities;`)
- **Classes/Methods/Properties**: `PascalCase`
- **Private fields**: `_camelCase` (underscore prefix)
- **Parameters/locals**: `camelCase`
- **Async methods**: Suffix with `Async` (e.g., `StartOnboardingAsync`)
- **Interfaces**: Prefix with `I` (e.g., `ITemplateRepository`)
- **Permissions**: GCP-style `{domain}.{plural-resource}.{action}` as `public const string` in a `Permissions` static class
  - Valid: `lifecycle.onboardings.create`, `lifecycle.templates.update`
  - Invalid: `lifecycle.onboarding.create` (singular), `lifecycle.create` (missing resource)
- **XML docs**: Required on ALL public methods and properties
- **Nullable**: Enabled (`<Nullable>enable</Nullable>`). Use `?` explicitly
- **Imports**: System first, then third-party, then local. Alphabetize within groups. Remove unused `using`
- **Braces**: Allman style (new line) for methods and control structures. Expression-bodied for properties/accessors
- **Indentation**: 4 spaces, LF line endings, UTF-8, trim trailing whitespace

### C# Patterns
- **DI**: Constructor injection with `private readonly` fields
- **Controllers**: `[ApiController]`, `[ApiVersion("1")]`, `[Route("lifecycle/v{version:apiVersion}")]`
- **Logging**: `ILogger<T>` with structured placeholders (never interpolate): `_logger.LogInformation("Processing {OnboardingId}", onboardingId)`
- **Error handling**: Global exception middleware. Return `ProblemDetails` / `ErrorResponse` DTOs. Never expose stack traces
- **JSON**: Check existing conventions in this service for naming policy
- **Manual mapping**: Static extension methods (`ToDto()`, `ToEntity()`). AutoMapper is banned
- **Validation**: `System.ComponentModel.DataAnnotations` on DTOs. FluentValidation is banned

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

---

## Banned Libraries (Build Will Fail)

| Banned | Use Instead |
|--------|-------------|
| AutoMapper | Manual mapping extensions |
| FluentValidation | DataAnnotations or manual validation |
| FluentAssertions | Standard xUnit `Assert.*` |
| Swashbuckle/Swagger | Scalar (at `/lifecycle/scalar`) |
| InMemoryDatabase (EF Core) | Testcontainers with real PostgreSQL |

---

## Testing Rules

- **Framework**: xUnit with standard `Assert` (`Assert.Equal`, `Assert.NotNull`, etc.)
- **Mocking**: Use **Moq** for unit tests
- **Naming**: `MethodName_StateUnderTest_ExpectedBehavior` or `HTTP_METHOD_Path_Scenario_ExpectedStatus`
- **Coverage**: Minimum 80% per service
- **Integration tests**: `BaseIntegrationTestFactory<TProgram, TDbContext>` with Testcontainers (PostgreSQL, Redis, RabbitMQ). Never InMemoryDatabase
- **System tests** (Tier 3): `AspireTestFixture` with `[Collection("AspireDomainTests")]` — shared AppHost, never one per class
- **Eventual consistency**: Use `TestHelpers.WaitForAsync`. Never `Task.Delay`
- **MassTransit consumers**: Must have consumer tests using `AddMassTransitTestHarness()`
- Use `[Fact]` for single cases, `[Theory]` for parameterized tests
- Use `// Arrange`, `// Act`, `// Assert` comments in test bodies

### Testing Strategy (4-Tier Pyramid Context)

This service's tests cover **Tier 1 (Unit)** and **Tier 2 (Service Integration)** of the Maliev testing pyramid:

| Tier | What to Test | Infrastructure |
|------|-------------|---------------|
| **Unit** | Business logic, domain models, service methods with mocked dependencies | None (mocks only) |
| **Service Integration** | API endpoints, database persistence, permission enforcement, input validation | `BaseIntegrationTestFactory` + Testcontainers (Postgres/Redis/RabbitMQ) |

**Tier 3 (System Integration)** — cross-service workflows and event chains — is tested in `Maliev.Aspire.Tests/`.

> Full ecosystem test strategy: `Maliev.Aspire.Tests/TEST_PLAN.md`

---

## Dependencies & Tools
- **ORM**: Entity Framework Core (Npgsql).
- **Messaging**: MassTransit (RabbitMQ).
- **Caching**: StackExchange.Redis.
- **Testing**: xUnit, Moq, Testcontainers.

---

## Mandatory Rules

- **`TreatWarningsAsErrors = true`**: Zero warnings allowed. No suppression
- **`[RequirePermission("lifecycle.resources.action")]`**: On all endpoints, not plain `[Authorize]`
- **API versioning**: All routes versioned (`v1/`)
- **Service prefix**: Routes prefixed with service domain (`/lifecycle`)
- **Scalar docs**: Configured at `/lifecycle/scalar`
- **Secrets**: Never hardcoded. Use GCP Secret Manager or environment variables
- **Async/await**: All the way down. Pass `CancellationToken`
- **EF Core Design package**: Only in Infrastructure project, never in Api
- **PostgreSQL xmin**: Shadow property only — `entity.Property<uint>("xmin").HasColumnType("xid").IsRowVersion()`. Never add entity property
- **Temporary files**: Generate in `/temp` folder, clean up afterwards

---

## Git Rules

- Each `Maliev.*` folder is an independent git repo. `cd` into it before git commands
- **Commit early and often** after every meaningful unit of work. Do not accumulate changes
- **Never use `git checkout` to restore files** — commit first, then `git revert` or `git reset --soft`
- Feature branches merged to `develop` via PR. Do not push without being asked

---

## Database & EF Core — Mandatory Rules

### EF Core Design Package
- ❌ `Microsoft.EntityFrameworkCore.Design` MUST NOT be in Api projects
- ✅ It belongs ONLY in the Infrastructure (or Data) project where migrations live
- Migration commands must target Infrastructure as both project and startup-project:
  ```
  dotnet ef migrations add <Name> --project Maliev.LifecycleService.Infrastructure --startup-project Maliev.LifecycleService.Infrastructure
  ```

### PostgreSQL xmin Concurrency — Mandatory Pattern
Use shadow property ONLY. Never add a Xmin/xmin property to domain entities.
```csharp
entity.Property<uint>("xmin").HasColumnType("xid").IsRowVersion();
```
- ❌ Never use `UseXminAsConcurrencyToken()` (removed in Npgsql EF v7)
- ❌ Never use entity property `public uint Xmin { get; set; }` or `public uint xmin { get; set; }`
- ❌ Never use `.Ignore(e => e.Xmin)` — remove the entity property instead

---

## AI Rules (Implicit)
- **Modifications**: When modifying code, preserve existing style and conventions.
- **Safety**: Do not commit secrets.
- **Verification**: Always compile and run relevant tests before confirming a task.

Instructions from: C:\Users\natth\.claude\CLAUDE.md
Always verify any changes you made with successful build. Never assume any changes you made will not result in broken build.
