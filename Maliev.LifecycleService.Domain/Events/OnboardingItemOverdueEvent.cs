namespace Maliev.LifecycleService.Domain.Events;

/// <summary>
/// Event published when an onboarding task exceeds its due date.
/// Trigger for escalation notifications.
/// </summary>
/// <param name="ChecklistId">The unique identifier of the associated onboarding checklist.</param>
/// <param name="ItemId">The unique identifier of the overdue item.</param>
/// <param name="ItemTitle">The title of the overdue item.</param>
/// <param name="AssignedTo">The identifier of the user or role assigned to the item.</param>
/// <param name="DueDate">The date and time when the item was due.</param>
public record OnboardingItemOverdueEvent(
    Guid ChecklistId,
    Guid ItemId,
    string ItemTitle,
    Guid? AssignedTo,
    DateTime DueDate);