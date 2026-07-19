namespace Maliev.LifecycleService.Application.Interfaces;

/// <summary>
/// Defines the service for recording business metrics related to the employee lifecycle.
/// </summary>
public interface ILifecycleMetrics
{
    /// <summary>
    /// Records that an onboarding process has started.
    /// </summary>
    void RecordOnboardingStarted();

    /// <summary>
    /// Records that an onboarding process has been completed.
    /// </summary>
    void RecordOnboardingCompleted();

    /// <summary>
    /// Records that an offboarding process has started.
    /// </summary>
    void RecordOffboardingStarted();

    /// <summary>
    /// Records that an offboarding process has been completed.
    /// </summary>
    void RecordOffboardingCompleted();

    /// <summary>
    /// Records that a task has become overdue.
    /// </summary>
    void RecordTaskOverdue();

    /// <summary>
    /// Records that an exit interview has been recorded.
    /// </summary>
    void RecordExitInterviewRecorded();

    /// <summary>
    /// Records the duration of an onboarding process in days.
    /// </summary>
    /// <param name="days">The number of days.</param>
    void RecordOnboardingDuration(double days);

    /// <summary>
    /// Records the duration of an offboarding process in days.
    /// </summary>
    /// <param name="days">The number of days.</param>
    void RecordOffboardingDuration(double days);

    /// <summary>
    /// Records a template cache hit or miss.
    /// </summary>
    /// <param name="hit">True if cache hit, false if miss.</param>
    void RecordTemplateCacheAccess(bool hit);
}
