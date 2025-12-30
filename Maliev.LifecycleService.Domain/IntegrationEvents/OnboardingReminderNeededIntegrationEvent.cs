namespace Maliev.LifecycleService.Domain.IntegrationEvents;

/// <summary>
/// Event published when an onboarding task is overdue and requires a reminder.
/// </summary>
public record OnboardingReminderNeededIntegrationEvent(
    Guid EmployeeId,
    Guid ItemId,
    string ItemTitle,
    Guid? AssignedTo);
