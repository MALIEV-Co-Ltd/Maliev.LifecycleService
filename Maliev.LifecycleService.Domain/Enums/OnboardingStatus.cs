namespace Maliev.LifecycleService.Domain.Enums;

/// <summary>
/// Represents the status of an onboarding checklist.
/// </summary>
public enum OnboardingStatus
{
    /// <summary>
    /// Checklist created but employee hasn't started
    /// </summary>
    NotStarted = 0,

    /// <summary>
    /// At least one item completed, not all complete
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// All items completed
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Onboarding aborted (employee didn't join)
    /// </summary>
    Cancelled = 3
}
