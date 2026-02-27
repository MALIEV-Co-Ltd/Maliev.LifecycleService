using Maliev.LifecycleService.Domain.Enums;

namespace Maliev.LifecycleService.Domain.Entities;

/// <summary>
/// Represents a checklist for managing an employee's onboarding process.
/// </summary>
public class OnboardingChecklist
{
    /// <summary>
    /// Gets or sets the unique identifier for the onboarding checklist.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the employee being onboarded.
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Gets or sets the current status of the onboarding process.
    /// </summary>
    public OnboardingStatus Status { get; set; } = OnboardingStatus.NotStarted;

    /// <summary>
    /// Gets or sets the scheduled start date for the employee.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the date when the onboarding process was completed.
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Gets or sets the total number of items in the onboarding checklist.
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Gets or sets the number of completed items in the onboarding checklist.
    /// </summary>
    public int CompletedItems { get; set; }

    /// <summary>
    /// Gets or sets the date when the record was created.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date when the record was last modified.
    /// </summary>
    public DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// Gets or sets the collection of items associated with this onboarding checklist.
    /// </summary>
    public ICollection<OnboardingItem> Items { get; set; } = new List<OnboardingItem>();
}
