using Maliev.LifecycleService.Domain.Entities;

namespace Maliev.LifecycleService.Application.Services;

/// <summary>
/// Defines the service for calculating if an employee's paycheck should be blocked based on offboarding tasks.
/// </summary>
public interface IPaycheckBlockingService
{
    /// <summary>
    /// Calculates whether the final paycheck release should be blocked for the given offboarding checklist.
    /// </summary>
    /// <param name="checklist">The offboarding checklist to evaluate.</param>
    /// <returns>True if the paycheck should be blocked; otherwise, false.</returns>
    bool CalculatePaycheckBlocked(OffboardingChecklist checklist);
}

/// <summary>
/// Implementation of the paycheck blocking calculation logic.
/// </summary>
public class PaycheckBlockingService : IPaycheckBlockingService
{
    /// <inheritdoc/>
    public bool CalculatePaycheckBlocked(OffboardingChecklist checklist)
    {
        // If all paycheck blocker tasks are completed, then paycheck is NOT blocked.
        // Otherwise, it IS blocked.
        return checklist.Tasks.Any(t => t.IsPaycheckBlocker && !t.IsCompleted);
    }
}
