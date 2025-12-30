namespace Maliev.LifecycleService.Domain.Events;

/// <summary>
/// Event published when all onboarding items are completed.
/// </summary>
/// <param name="ChecklistId">The unique identifier of the completed onboarding checklist.</param>
/// <param name="EmployeeId">The unique identifier of the onboarded employee.</param>
/// <param name="CompletedDate">The date and time when onboarding was completed.</param>
public record OnboardingCompletedEvent(
    Guid ChecklistId,
    Guid EmployeeId,
    DateTime CompletedDate);