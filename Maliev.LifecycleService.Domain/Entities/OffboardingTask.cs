using Maliev.LifecycleService.Domain.Enums;

namespace Maliev.LifecycleService.Domain.Entities;

/// <summary>
/// Represents a specific task within an offboarding checklist.
/// </summary>
public class OffboardingTask
{
    /// <summary>
    /// Gets or sets the unique identifier for the offboarding task.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the associated offboarding checklist identifier.
    /// </summary>
    public Guid OffboardingChecklistId { get; set; }

    /// <summary>
    /// Gets or sets the title of the task.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a description of what the task entails.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the category of the offboarding task.
    /// </summary>
    public TaskCategory Category { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user or role assigned to the task.
    /// </summary>
    public Guid? AssignedTo { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this task blocks the final paycheck release.
    /// </summary>
    public bool IsPaycheckBlocker { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the task is completed.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Gets or sets the date when the task was completed.
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who completed the task.
    /// </summary>
    public Guid? CompletedBy { get; set; }

    /// <summary>
    /// Gets or sets additional notes regarding the task completion.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the order in which the task should be displayed.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets or sets the date when the record was created.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the associated offboarding checklist navigation property.
    /// </summary>
    public OffboardingChecklist Checklist { get; set; } = null!;
}
