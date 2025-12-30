namespace Maliev.LifecycleService.Domain.IntegrationEvents;

/// <summary>
/// Event published when an employee's access needs to be revoked (e.g., termination).
/// </summary>
public record AccessRevocationRequiredIntegrationEvent(
    Guid EmployeeId,
    DateTime TerminationDate);
