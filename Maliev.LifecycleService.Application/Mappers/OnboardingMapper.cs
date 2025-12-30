using Maliev.LifecycleService.Application.DTOs;
using Maliev.LifecycleService.Domain.Entities;

namespace Maliev.LifecycleService.Application.Mappers;

/// <summary>
/// Provides extension methods for mapping onboarding-related entities to DTOs.
/// </summary>
public static class OnboardingMapper
{
    /// <summary>
    /// Maps an <see cref="OnboardingChecklist"/> entity to an <see cref="OnboardingStatusDto"/>.
    /// </summary>
    /// <param name="checklist">The onboarding checklist entity to map.</param>
    /// <returns>The mapped onboarding status DTO.</returns>
    public static OnboardingStatusDto ToDto(this OnboardingChecklist checklist)
    {
        return new OnboardingStatusDto(
            checklist.Id,
            checklist.EmployeeId,
            checklist.Status,
            checklist.StartDate,
            checklist.CompletedDate,
            checklist.TotalItems,
            checklist.CompletedItems,
            checklist.Items.Select(ToDto).ToList()
        );
    }

    /// <summary>
    /// Maps an <see cref="OnboardingItem"/> entity to an <see cref="OnboardingItemDto"/>.
    /// </summary>
    /// <param name="item">The onboarding item entity to map.</param>
    /// <returns>The mapped onboarding item DTO.</returns>
    public static OnboardingItemDto ToDto(this OnboardingItem item)
    {
        return new OnboardingItemDto(
            item.Id,
            item.Title,
            item.Description,
            item.Category,
            item.AssignedTo,
            item.DaysDue,
            item.IsCompleted,
            item.CompletedDate,
            item.CompletedBy,
            item.Notes,
            item.SortOrder
        );
    }
}