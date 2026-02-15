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
- Validate inputs early using **FluentValidation** (if applicable) or Guard clauses.
- Controller actions should return standard HTTP responses (200, 201, 400, 404).

#### Testing
- **Framework**: xUnit.
- **Mocking**: Use **Moq** for unit tests.
- **Integration Tests**: Use `WebApplicationFactory` and Testcontainers (Postgres, RabbitMQ, Redis).
- **Structure**: Use `// Arrange`, `// Act`, `// Assert` comments in test bodies.

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
