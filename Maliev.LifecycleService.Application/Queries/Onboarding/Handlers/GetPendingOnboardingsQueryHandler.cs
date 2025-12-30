using Maliev.LifecycleService.Application.DTOs;
using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Application.Mappers;

namespace Maliev.LifecycleService.Application.Queries.Onboarding.Handlers;

/// <summary>
/// Handles the retrieval of a list of pending onboarding checklists.
/// </summary>
public class GetPendingOnboardingsQueryHandler
{
    private readonly IOnboardingRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPendingOnboardingsQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The onboarding repository.</param>
    public GetPendingOnboardingsQueryHandler(IOnboardingRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles the query to list pending onboardings.
    /// </summary>
    /// <param name="query">The get pending onboardings query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of onboarding status DTOs.</returns>
    public async Task<IEnumerable<OnboardingStatusDto>> HandleAsync(GetPendingOnboardingsQuery query, CancellationToken cancellationToken = default)
    {
        var checklists = await _repository.GetPendingAsync(query.Offset, query.Limit, cancellationToken);
        return checklists.Select(x => x.ToDto());
    }
}