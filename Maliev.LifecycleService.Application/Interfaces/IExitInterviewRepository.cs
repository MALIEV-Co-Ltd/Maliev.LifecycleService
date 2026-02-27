using Maliev.LifecycleService.Domain.Entities;

namespace Maliev.LifecycleService.Application.Interfaces;

/// <summary>
/// Defines the repository for persisting and retrieving exit interviews.
/// </summary>
public interface IExitInterviewRepository
{
    /// <summary>
    /// Adds a new exit interview.
    /// </summary>
    /// <param name="exitInterview">The exit interview to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(ExitInterview exitInterview, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an exit interview by employee identifier.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the employee.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The exit interview if found; otherwise, null.</returns>
    Task<ExitInterview?> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an exit interview by associated checklist identifier.
    /// </summary>
    /// <param name="checklistId">The unique identifier of the offboarding checklist.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The exit interview if found; otherwise, null.</returns>
    Task<ExitInterview?> GetByChecklistIdAsync(Guid checklistId, CancellationToken cancellationToken = default);
}
