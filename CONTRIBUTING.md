# Contributing to Maliev.LifecycleService

Thank you for contributing to the Lifecycle Service! This document outlines the development workflow and coding standards for this project.

## Development Workflow

1.  **Environment Setup**:
    *   Ensure you have .NET 10.0 SDK installed.
    *   Docker Desktop is required for running infrastructure dependencies via Testcontainers.
    *   Clone the repository and run `dotnet restore`.

2.  **Local Infrastructure**:
    *   You can start local dependencies using `docker compose up -d` if needed, but tests will automatically spin up required containers using Testcontainers.

3.  **Project Structure**:
    *   Follow Clean Architecture principles.
    *   **Domain**: Entities, Enums, Events, Exceptions.
    *   **Application**: Commands, Queries, DTOs, Handlers, Interfaces, Services, Mappers, Validators.
    *   **Infrastructure**: Data Access (EF Core), Repositories, Messaging (MassTransit), Background Services, Caching, IAM Integration.
    *   **Api**: Controllers, Middleware, Configuration.

4.  **Coding Standards**:
    *   **Zero Warnings**: All build warnings are treated as errors. Ensure your code compiles without warnings.
    *   **Naming**: Use PascalCase for classes, methods, and properties. Use camelCase for local variables and private fields (with `_` prefix). Use snake_case for database names (handled by `SnakeCaseNamingHelper`).
    *   **Documentation**: Provide XML documentation for all public members.
    *   **No External Mappers/Validators**: Use explicit mapping and DataAnnotations/manual validation. Do not use AutoMapper or FluentValidation.
    *   **Explicit Contracts**: All API endpoints must be documented and use DTOs for requests and responses.

5.  **Testing**:
    *   Follow Test-Driven Development (TDD).
    *   Write unit tests for application logic (handlers, services, validators).
    *   Write integration tests for repositories, background services, and controllers using Testcontainers.
    *   Do not use FluentAssertions; use standard xUnit `Assert`.

6.  **Pull Requests**:
    *   Ensure all tests pass before submitting a PR.
    *   Run `dotnet format` before committing.
    *   PRs must be reviewed by at least one core developer.

## CI/CD

*   `develop` branch → Automatic deployment to Development environment.
*   `staging` branch → Automatic deployment to Staging environment.
*   `main` branch → Automatic deployment to Production environment.
