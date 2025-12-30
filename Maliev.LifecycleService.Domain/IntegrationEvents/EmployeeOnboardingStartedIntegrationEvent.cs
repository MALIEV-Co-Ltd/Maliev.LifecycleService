namespace Maliev.LifecycleService.Domain.IntegrationEvents;

/// <summary>
/// Event published when an employee's onboarding process has officially started.
/// </summary>
public record EmployeeOnboardingStartedIntegrationEvent(
    Guid EmployeeId,
    DateTime StartDate,
    Guid TemplateId);
