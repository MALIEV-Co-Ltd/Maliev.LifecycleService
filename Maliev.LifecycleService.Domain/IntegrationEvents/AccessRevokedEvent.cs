namespace Maliev.LifecycleService.Domain.IntegrationEvents;

/// <summary>
/// Event published when an employee's system access has been successfully revoked.
/// </summary>
public record AccessRevokedEvent(Guid EmployeeId, Guid CorrelationId);
