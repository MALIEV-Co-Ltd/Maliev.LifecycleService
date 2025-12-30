namespace Maliev.LifecycleService.Domain.Events;

/// <summary>
/// Event published when IT access revocation is required (usually during offboarding).
/// </summary>
/// <param name="EmployeeId">The unique identifier of the employee whose access should be revoked.</param>
/// <param name="RequiredByDate">The date and time by which the revocation should be completed.</param>
/// <param name="Reason">The reason for the access revocation request.</param>
public record AccessRevocationRequiredEvent(
    Guid EmployeeId,
    DateTime RequiredByDate,
    string Reason);