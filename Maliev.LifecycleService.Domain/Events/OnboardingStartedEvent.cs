namespace Maliev.LifecycleService.Domain.Events;

/// <summary>
/// Event published when an onboarding checklist is initialized.
/// </summary>
/// <param name="ChecklistId">The unique identifier of the started onboarding checklist.</param>
/// <param name="EmployeeId">The unique identifier of the employee being onboarded.</param>
/// <param name="StartDate">The employee's start date.</param>
public record OnboardingStartedEvent(
    Guid ChecklistId,
    Guid EmployeeId,
    DateTime StartDate);