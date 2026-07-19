using Maliev.LifecycleService.Domain.Entities;

namespace Maliev.LifecycleService.Application.Interfaces;

/// <summary>
/// Defines the repository for persisting and retrieving onboarding templates.
/// </summary>
public interface ITemplateRepository
{
    /// <summary>
    /// Retrieves an onboarding template with its items by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the template.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The onboarding template with items if found; otherwise, null.</returns>
    Task<OnboardingTemplate?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the active onboarding template for a department or the default template.
    /// </summary>
    /// <param name="departmentId">The optional department identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The onboarding template if found; otherwise, null.</returns>
    Task<OnboardingTemplate?> GetByDepartmentIdAsync(Guid? departmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all onboarding templates.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of all onboarding templates.</returns>
    Task<IEnumerable<OnboardingTemplate>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new onboarding template.
    /// </summary>
    /// <param name="template">The onboarding template to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(OnboardingTemplate template, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing onboarding template.
    /// </summary>
    /// <param name="template">The onboarding template to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(OnboardingTemplate template, CancellationToken cancellationToken = default);
}
