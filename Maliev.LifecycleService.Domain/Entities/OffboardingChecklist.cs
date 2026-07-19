using Maliev.LifecycleService.Domain.Enums;

namespace Maliev.LifecycleService.Domain.Entities;

/// <summary>
/// Represents a checklist for managing an employee's offboarding process.
/// </summary>
public class OffboardingChecklist
{
    /// <summary>
    /// Gets or sets the unique identifier for the offboarding checklist.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the employee being offboarded.
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Gets or sets the scheduled termination date.
    /// </summary>
    public DateTime TerminationDate { get; set; }

    /// <summary>
    /// Gets or sets the reason for termination.
    /// </summary>
    public string TerminationReason { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the employee is eligible for rehire.
    /// </summary>
    public bool EligibleForRehire { get; set; } = true;

    /// <summary>
    /// Gets or sets the current status of the offboarding process.
    /// </summary>
    public OffboardingStatus Status { get; set; } = OffboardingStatus.NotStarted;

    /// <summary>
    /// Gets or sets a value indicating whether the final paycheck release is currently blocked.
    /// </summary>
    public bool PaycheckReleaseBlocked { get; set; } = true;

    /// <summary>
    /// Gets or sets the date when the offboarding process was completed.
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Gets or sets the date when the record was created.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date when the record was last modified.
    /// </summary>
    public DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// Gets or sets the collection of tasks associated with this offboarding checklist.
    /// </summary>
    public ICollection<OffboardingTask> Tasks { get; set; } = new List<OffboardingTask>();
}
