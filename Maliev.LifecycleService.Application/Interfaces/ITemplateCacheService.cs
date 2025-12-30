using Maliev.LifecycleService.Domain.Entities;

namespace Maliev.LifecycleService.Application.Interfaces;

/// <summary>
/// Defines the distributed cache service for onboarding templates.
/// </summary>
public interface ITemplateCacheService
{
    /// <summary>
    /// Retrieves a template from the cache.
    /// </summary>
    /// <param name="departmentId">The department identifier used as a cache key part.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The cached onboarding template if found; otherwise, null.</returns>
    Task<OnboardingTemplate?> GetAsync(Guid? departmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores a template in the cache.
    /// </summary>
    /// <param name="departmentId">The department identifier used as a cache key part.</param>
    /// <param name="template">The template to cache.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetAsync(Guid? departmentId, OnboardingTemplate template, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a template from the cache.
    /// </summary>
    /// <param name="departmentId">The department identifier used as a cache key part.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveAsync(Guid? departmentId, CancellationToken cancellationToken = default);
}