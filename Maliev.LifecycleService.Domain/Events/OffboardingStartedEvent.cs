namespace Maliev.LifecycleService.Domain.Events;

/// <summary>
/// Event published when an offboarding workflow starts.
/// </summary>
/// <param name="ChecklistId">The unique identifier of the started offboarding checklist.</param>
/// <param name="EmployeeId">The unique identifier of the employee being offboarded.</param>
/// <param name="TerminationDate">The scheduled termination date.</param>
public record OffboardingStartedEvent(
    Guid ChecklistId,
    Guid EmployeeId,
    DateTime TerminationDate);