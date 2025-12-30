namespace Maliev.LifecycleService.Domain.Events;

/// <summary>
/// Event published when offboarding is finalized.
/// </summary>
/// <param name="ChecklistId">The unique identifier of the completed offboarding checklist.</param>
/// <param name="EmployeeId">The unique identifier of the offboarded employee.</param>
/// <param name="CompletedDate">The date and time when offboarding was completed.</param>
public record OffboardingCompletedEvent(
    Guid ChecklistId,
    Guid EmployeeId,
    DateTime CompletedDate);