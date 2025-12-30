namespace Maliev.LifecycleService.Domain.Entities;

/// <summary>
/// Represents an exit interview recorded during the offboarding process.
/// </summary>
public class ExitInterview
{
    /// <summary>
    /// Gets or sets the unique identifier for the exit interview.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the associated offboarding checklist identifier.
    /// </summary>
    public Guid OffboardingChecklistId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who conducted the interview.
    /// </summary>
    public Guid ConductedBy { get; set; }

    /// <summary>
    /// Gets or sets the date when the interview was conducted.
    /// </summary>
    public DateTime InterviewDate { get; set; }

    /// <summary>
    /// Gets or sets the reason provided for leaving.
    /// </summary>
    public string? ReasonForLeaving { get; set; }

    /// <summary>
    /// Gets or sets feedback regarding the manager.
    /// </summary>
    public string? FeedbackOnManager { get; set; }

    /// <summary>
    /// Gets or sets feedback regarding the team.
    /// </summary>
    public string? FeedbackOnTeam { get; set; }

    /// <summary>
    /// Gets or sets feedback regarding the company.
    /// </summary>
    public string? FeedbackOnCompany { get; set; }

    /// <summary>
    /// Gets or sets suggestions for improvement.
    /// </summary>
    public string? ImprovementSuggestions { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the employee would recommend the company.
    /// </summary>
    public bool WouldRecommendCompany { get; set; } = true;

    /// <summary>
    /// Gets or sets the date when the record was created.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the associated offboarding checklist navigation property.
    /// </summary>
    public OffboardingChecklist OffboardingChecklist { get; set; } = null!;
}