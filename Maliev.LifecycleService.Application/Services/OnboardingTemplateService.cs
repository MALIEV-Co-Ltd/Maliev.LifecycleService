using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;

namespace Maliev.LifecycleService.Application.Services;

/// <summary>
/// Provides methods for managing onboarding templates with caching support.
/// </summary>
public class OnboardingTemplateService : IOnboardingTemplateService
{
    private readonly ITemplateRepository _repository;
    private readonly ITemplateCacheService _cache;
    private readonly ILifecycleMetrics _metrics;

    /// <summary>
    /// Initializes a new instance of the <see cref="OnboardingTemplateService"/> class.
    /// </summary>
    /// <param name="repository">The template repository.</param>
    /// <param name="cache">The template cache service.</param>
    /// <param name="metrics">The lifecycle metrics service.</param>
    public OnboardingTemplateService(ITemplateRepository repository, ITemplateCacheService cache, ILifecycleMetrics metrics)
    {
        _repository = repository;
        _cache = cache;
        _metrics = metrics;
    }

    /// <inheritdoc/>
    public async Task<OnboardingTemplate?> GetTemplateForDepartmentAsync(Guid? departmentId, CancellationToken cancellationToken = default)
    {
        // 1. Try Cache
        var cached = await _cache.GetAsync(departmentId, cancellationToken);
        if (cached != null)
        {
            _metrics.RecordTemplateCacheAccess(true);
            return cached;
        }

        _metrics.RecordTemplateCacheAccess(false);

        // 2. Try DB
        var template = await _repository.GetByDepartmentIdAsync(departmentId, cancellationToken);

        // 3. Set Cache only if we found an exact match for the department to avoid pollution
        if (template != null && template.DepartmentId == departmentId)
        {
            await _cache.SetAsync(departmentId, template, cancellationToken);
        }

        return template;
    }
}
