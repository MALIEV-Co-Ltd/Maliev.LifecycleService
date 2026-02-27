using Maliev.LifecycleService.Application.DTOs;
using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Application.Mappers;

namespace Maliev.LifecycleService.Application.Queries.Offboarding.Handlers;

/// <summary>
/// Handles the retrieval of offboarding status for an employee.
/// </summary>
public class GetOffboardingStatusQueryHandler
{
    private readonly IOffboardingRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetOffboardingStatusQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The offboarding repository.</param>
    public GetOffboardingStatusQueryHandler(IOffboardingRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles the query to get offboarding status.
    /// </summary>
    /// <param name="query">The get offboarding status query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The offboarding status DTO if found; otherwise, null.</returns>
    public async Task<OffboardingStatusDto?> HandleAsync(GetOffboardingStatusQuery query, CancellationToken cancellationToken = default)
    {
        var checklist = await _repository.GetByEmployeeIdAsync(query.EmployeeId, cancellationToken);
        return checklist?.ToDto();
    }
}
