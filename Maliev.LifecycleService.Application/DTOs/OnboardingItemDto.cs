using Maliev.LifecycleService.Domain.Enums;

namespace Maliev.LifecycleService.Application.DTOs;

/// <summary>
/// Data transfer object for a specific onboarding item.
/// </summary>
/// <param name="Id">The unique identifier of the onboarding item.</param>
/// <param name="Title">The title of the onboarding item.</param>
/// <param name="Description">A description of what the onboarding item entails.</param>
/// <param name="Category">The category of the onboarding item.</param>
/// <param name="AssignedTo">The identifier of the user or role assigned to the item.</param>
/// <param name="DaysDue">Number of days after start date that this item is due.</param>
/// <param name="IsCompleted">A value indicating whether the onboarding item is completed.</param>
/// <param name="CompletedDate">The date when the item was completed.</param>
/// <param name="CompletedBy">The identifier of the user who completed the item.</param>
/// <param name="Notes">Additional notes regarding the item completion.</param>
/// <param name="SortOrder">The order in which the item should be displayed.</param>
public record OnboardingItemDto(
    Guid Id,
    string Title,
    string? Description,
    ItemCategory Category,
    Guid? AssignedTo,
    int DaysDue,
    bool IsCompleted,
    DateTime? CompletedDate,
    Guid? CompletedBy,
    string? Notes,
    int SortOrder);