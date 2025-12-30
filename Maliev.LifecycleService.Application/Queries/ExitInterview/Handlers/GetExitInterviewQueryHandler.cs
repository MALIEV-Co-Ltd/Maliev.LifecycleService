using Maliev.LifecycleService.Application.DTOs;
using Maliev.LifecycleService.Application.Interfaces;

namespace Maliev.LifecycleService.Application.Queries.ExitInterview.Handlers;

/// <summary>
/// Handles the retrieval of exit interview information.
/// </summary>
public class GetExitInterviewQueryHandler
{
    private readonly IExitInterviewRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetExitInterviewQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The exit interview repository.</param>
    public GetExitInterviewQueryHandler(IExitInterviewRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles the query to get an exit interview.
    /// </summary>
    /// <param name="query">The get exit interview query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The exit interview DTO if found; otherwise, null.</returns>
    public async Task<ExitInterviewDto?> HandleAsync(GetExitInterviewQuery query, CancellationToken cancellationToken = default)
    {
        var exitInterview = await _repository.GetByEmployeeIdAsync(query.EmployeeId, cancellationToken);
        if (exitInterview == null) return null;

        return new ExitInterviewDto(
            exitInterview.Id,
            exitInterview.OffboardingChecklistId,
            exitInterview.ConductedBy,
            exitInterview.InterviewDate,
            exitInterview.ReasonForLeaving,
            exitInterview.FeedbackOnManager,
            exitInterview.FeedbackOnTeam,
            exitInterview.FeedbackOnCompany,
            exitInterview.ImprovementSuggestions,
            exitInterview.WouldRecommendCompany
        );
    }
}