using Maliev.LifecycleService.Domain.Enums;

namespace Maliev.LifecycleService.Domain.Entities;

/// <summary>
/// Represents a specific item within an onboarding checklist.
/// </summary>
public class OnboardingItem
{
    /// <summary>
    /// Gets or sets the unique identifier for the onboarding item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the associated onboarding checklist identifier.
    /// </summary>
    public Guid OnboardingChecklistId { get; set; }

    /// <summary>
    /// Gets or sets the title of the onboarding item.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a description of what the onboarding item entails.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the category of the onboarding item.
    /// </summary>
    public ItemCategory Category { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user or role assigned to the item.
    /// </summary>
    public Guid? AssignedTo { get; set; }

    /// <summary>
    /// Gets or sets the number of days after the start date that this item is due.
    /// </summary>
    public int DaysDue { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the onboarding item is completed.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Gets or sets the date when the item was completed.
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who completed the item.
    /// </summary>
    public Guid? CompletedBy { get; set; }

    /// <summary>
    /// Gets or sets additional notes regarding the item completion.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the order in which the item should be displayed.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets or sets the date when the record was created.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the associated onboarding checklist navigation property.
    /// </summary>
    public OnboardingChecklist Checklist { get; set; } = null!;
}