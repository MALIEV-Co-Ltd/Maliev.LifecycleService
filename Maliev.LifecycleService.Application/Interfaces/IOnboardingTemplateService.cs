using Maliev.LifecycleService.Domain.Entities;

namespace Maliev.LifecycleService.Application.Interfaces;

/// <summary>
/// Defines the service for managing onboarding templates.
/// </summary>
public interface IOnboardingTemplateService
{
    /// <summary>
    /// Retrieves the most appropriate onboarding template for a department.
    /// </summary>
    /// <param name="departmentId">The optional department identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The onboarding template if found; otherwise, null.</returns>
    Task<OnboardingTemplate?> GetTemplateForDepartmentAsync(Guid? departmentId, CancellationToken cancellationToken = default);
}
