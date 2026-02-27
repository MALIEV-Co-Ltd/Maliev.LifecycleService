using Maliev.LifecycleService.Domain.Entities;

namespace Maliev.LifecycleService.Application.Interfaces;

/// <summary>
/// Defines the repository for persisting and retrieving onboarding checklists.
/// </summary>
public interface IOnboardingRepository
{
    /// <summary>
    /// Adds a new onboarding checklist.
    /// </summary>
    /// <param name="checklist">The onboarding checklist to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(OnboardingChecklist checklist, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing onboarding checklist.
    /// </summary>
    /// <param name="checklist">The onboarding checklist to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(OnboardingChecklist checklist, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an onboarding checklist by employee identifier.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the employee.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The onboarding checklist if found; otherwise, null.</returns>
    Task<OnboardingChecklist?> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an onboarding checklist with its items by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the onboarding checklist.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The onboarding checklist with items if found; otherwise, null.</returns>
    Task<OnboardingChecklist?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of pending onboarding checklists.
    /// </summary>
    /// <param name="offset">The number of items to skip.</param>
    /// <param name="limit">The maximum number of items to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of pending onboarding checklists.</returns>
    Task<IEnumerable<OnboardingChecklist>> GetPendingAsync(int offset, int limit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an onboarding item by its identifier.
    /// </summary>
    /// <param name="itemId">The unique identifier of the onboarding item.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The onboarding item if found; otherwise, null.</returns>
    Task<OnboardingItem?> GetItemByIdAsync(Guid itemId, CancellationToken cancellationToken = default);
}
