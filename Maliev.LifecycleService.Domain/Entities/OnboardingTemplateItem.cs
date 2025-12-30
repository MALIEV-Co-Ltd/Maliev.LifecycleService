using Maliev.LifecycleService.Domain.Enums;

namespace Maliev.LifecycleService.Domain.Entities;

/// <summary>
/// Represents a specific item within an onboarding template.
/// </summary>
public class OnboardingTemplateItem
{
    /// <summary>
    /// Gets or sets the unique identifier for the template item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the associated onboarding template identifier.
    /// </summary>
    public Guid TemplateId { get; set; }

    /// <summary>
    /// Gets or sets the title of the template item.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a description of what the template item entails.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the category of the onboarding item.
    /// </summary>
    public ItemCategory Category { get; set; }

    /// <summary>
    /// Gets or sets the default role assigned to this item when a checklist is generated.
    /// </summary>
    public string? DefaultAssigneeRole { get; set; }

    /// <summary>
    /// Gets or sets the number of days after the start date that this item is typically due.
    /// </summary>
    public int DaysDue { get; set; }

    /// <summary>
    /// Gets or sets the order in which the item should be displayed.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets or sets the associated onboarding template navigation property.
    /// </summary>
    public OnboardingTemplate Template { get; set; } = null!;
}