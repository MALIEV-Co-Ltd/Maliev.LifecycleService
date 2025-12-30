using Maliev.LifecycleService.Application.DTOs;
using Maliev.LifecycleService.Domain.Entities;

namespace Maliev.LifecycleService.Application.Mappers;

/// <summary>
/// Provides extension methods for mapping offboarding-related entities to DTOs.
/// </summary>
public static class OffboardingMapper
{
    /// <summary>
    /// Maps an <see cref="OffboardingChecklist"/> entity to an <see cref="OffboardingStatusDto"/>.
    /// </summary>
    /// <param name="checklist">The offboarding checklist entity to map.</param>
    /// <returns>The mapped offboarding status DTO.</returns>
    public static OffboardingStatusDto ToDto(this OffboardingChecklist checklist)
    {
        return new OffboardingStatusDto(
            checklist.Id,
            checklist.EmployeeId,
            checklist.TerminationDate,
            checklist.TerminationReason,
            checklist.EligibleForRehire,
            checklist.Status,
            checklist.PaycheckReleaseBlocked,
            checklist.CompletedDate,
            checklist.Tasks.Select(ToDto).ToList()
        );
    }

    /// <summary>
    /// Maps an <see cref="OffboardingTask"/> entity to an <see cref="OffboardingTaskDto"/>.
    /// </summary>
    /// <param name="task">The offboarding task entity to map.</param>
    /// <returns>The mapped offboarding task DTO.</returns>
    public static OffboardingTaskDto ToDto(this OffboardingTask task)
    {
        return new OffboardingTaskDto(
            task.Id,
            task.Title,
            task.Description,
            task.Category,
            task.AssignedTo,
            task.IsPaycheckBlocker,
            task.IsCompleted,
            task.CompletedDate,
            task.CompletedBy,
            task.Notes,
            task.SortOrder
        );
    }
}