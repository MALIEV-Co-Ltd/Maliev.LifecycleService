using Maliev.LifecycleService.Application.DTOs;
using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Application.Mappers;

namespace Maliev.LifecycleService.Application.Queries.Offboarding.Handlers;

/// <summary>
/// Handles the retrieval of a list of pending offboarding workflows.
/// </summary>
public class GetPendingOffboardingsQueryHandler
{
    private readonly IOffboardingRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPendingOffboardingsQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The offboarding repository.</param>
    public GetPendingOffboardingsQueryHandler(IOffboardingRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles the query to list pending offboardings.
    /// </summary>
    /// <param name="query">The get pending offboardings query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of offboarding status DTOs.</returns>
    public async Task<IEnumerable<OffboardingStatusDto>> HandleAsync(GetPendingOffboardingsQuery query, CancellationToken cancellationToken = default)
    {
        var checklists = await _repository.GetPendingAsync(query.Offset, query.Limit, cancellationToken);
        return checklists.Select(x => x.ToDto());
    }
}