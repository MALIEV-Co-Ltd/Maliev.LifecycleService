namespace Maliev.LifecycleService.Domain.Entities;

/// <summary>
/// Represents a template for creating onboarding checklists.
/// </summary>
public class OnboardingTemplate
{
    /// <summary>
    /// Gets or sets the unique identifier for the onboarding template.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the template.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a description of the template.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the optional department identifier this template is specific to.
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the template is active and can be used.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the date when the record was created.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date when the record was last modified.
    /// </summary>
    public DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// Gets or sets the collection of template items associated with this onboarding template.
    /// </summary>
    public ICollection<OnboardingTemplateItem> Items { get; set; } = new List<OnboardingTemplateItem>();
}
