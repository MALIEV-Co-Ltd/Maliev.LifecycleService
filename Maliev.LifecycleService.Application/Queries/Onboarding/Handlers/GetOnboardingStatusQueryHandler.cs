using Maliev.LifecycleService.Application.DTOs;
using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Application.Mappers;

namespace Maliev.LifecycleService.Application.Queries.Onboarding.Handlers;

/// <summary>
/// Handles the retrieval of onboarding status for an employee.
/// </summary>
public class GetOnboardingStatusQueryHandler
{
    private readonly IOnboardingRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetOnboardingStatusQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The onboarding repository.</param>
    public GetOnboardingStatusQueryHandler(IOnboardingRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles the query to get onboarding status.
    /// </summary>
    /// <param name="query">The get onboarding status query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The onboarding status DTO if found; otherwise, null.</returns>
    public async Task<OnboardingStatusDto?> HandleAsync(GetOnboardingStatusQuery query, CancellationToken cancellationToken = default)
    {
        var checklist = await _repository.GetByEmployeeIdAsync(query.EmployeeId, cancellationToken);
        return checklist?.ToDto();
    }
}