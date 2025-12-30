namespace Maliev.LifecycleService.Domain.Events;

/// <summary>
/// Event published when an employee is terminated.
/// Trigger for offboarding workflow.
/// </summary>
/// <param name="EmployeeId">The unique identifier of the terminated employee.</param>
/// <param name="TerminationDate">The date and time of termination.</param>
/// <param name="TerminationReason">The reason for termination.</param>
/// <param name="EligibleForRehire">A value indicating whether the employee is eligible for rehire.</param>
public record EmployeeTerminatedEvent(
    Guid EmployeeId,
    DateTime TerminationDate,
    string? TerminationReason,
    bool EligibleForRehire);