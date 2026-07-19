using Maliev.LifecycleService.Domain.Entities;

namespace Maliev.LifecycleService.Application.Interfaces;

/// <summary>
/// Defines the repository for persisting and retrieving offboarding checklists.
/// </summary>
public interface IOffboardingRepository
{
    /// <summary>
    /// Adds a new offboarding checklist.
    /// </summary>
    /// <param name="checklist">The offboarding checklist to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(OffboardingChecklist checklist, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing offboarding checklist.
    /// </summary>
    /// <param name="checklist">The offboarding checklist to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(OffboardingChecklist checklist, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an offboarding checklist by employee identifier.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the employee.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The offboarding checklist if found; otherwise, null.</returns>
    Task<OffboardingChecklist?> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an offboarding checklist with its tasks by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the offboarding checklist.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The offboarding checklist with tasks if found; otherwise, null.</returns>
    Task<OffboardingChecklist?> GetByIdWithTasksAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of pending offboarding checklists.
    /// </summary>
    /// <param name="offset">The number of items to skip.</param>
    /// <param name="limit">The maximum number of items to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of pending offboarding checklists.</returns>
    Task<IEnumerable<OffboardingChecklist>> GetPendingAsync(int offset, int limit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an offboarding task by its identifier.
    /// </summary>
    /// <param name="taskId">The unique identifier of the offboarding task.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The offboarding task if found; otherwise, null.</returns>
    Task<OffboardingTask?> GetTaskByIdAsync(Guid taskId, CancellationToken cancellationToken = default);
}
