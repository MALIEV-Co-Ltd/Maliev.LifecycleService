using Maliev.LifecycleService.Domain.Enums;

namespace Maliev.LifecycleService.Application.DTOs;

/// <summary>
/// Data transfer object for a specific offboarding task.
/// </summary>
/// <param name="Id">The unique identifier of the offboarding task.</param>
/// <param name="Title">The title of the task.</param>
/// <param name="Description">A description of what the task entails.</param>
/// <param name="Category">The category of the offboarding task.</param>
/// <param name="AssignedTo">The identifier of the user or role assigned to the task.</param>
/// <param name="IsPaycheckBlocker">A value indicating whether this task blocks the final paycheck release.</param>
/// <param name="IsCompleted">A value indicating whether the task is completed.</param>
/// <param name="CompletedDate">The date when the task was completed.</param>
/// <param name="CompletedBy">The identifier of the user who completed the task.</param>
/// <param name="Notes">Additional notes regarding the task completion.</param>
/// <param name="SortOrder">The display order of the task.</param>
public record OffboardingTaskDto(
    Guid Id,
    string Title,
    string? Description,
    TaskCategory Category,
    Guid? AssignedTo,
    bool IsPaycheckBlocker,
    bool IsCompleted,
    DateTime? CompletedDate,
    Guid? CompletedBy,
    string? Notes,
    int SortOrder);