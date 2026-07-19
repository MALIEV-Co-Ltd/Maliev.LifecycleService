namespace Maliev.LifecycleService.Domain.Enums;

/// <summary>
/// Represents the status of an offboarding checklist.
/// </summary>
public enum OffboardingStatus
{
    /// <summary>
    /// Checklist created but not yet in progress
    /// </summary>
    NotStarted = 0,

    /// <summary>
    /// At least one task completed, not all complete
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// All tasks completed
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Offboarding aborted (employee stayed)
    /// </summary>
    Cancelled = 3
}
