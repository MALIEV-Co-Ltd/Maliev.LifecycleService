using Maliev.LifecycleService.Application.Interfaces;

namespace Maliev.LifecycleService.Application.Commands.ExitInterview.Handlers;

/// <summary>
/// Handles the recording of an exit interview.
/// </summary>
public class RecordExitInterviewCommandHandler
{
    private readonly IExitInterviewRepository _exitInterviewRepository;
    private readonly IOffboardingRepository _offboardingRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly ILifecycleMetrics _metrics;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordExitInterviewCommandHandler"/> class.
    /// </summary>
    /// <param name="exitInterviewRepository">The repository for exit interviews.</param>
    /// <param name="offboardingRepository">The repository for offboarding checklists.</param>
    /// <param name="auditLogService">The service for recording audit logs.</param>
    /// <param name="metrics">The service for recording business metrics.</param>
    public RecordExitInterviewCommandHandler(
        IExitInterviewRepository exitInterviewRepository,
        IOffboardingRepository offboardingRepository,
        IAuditLogService auditLogService,
        ILifecycleMetrics metrics)
    {
        _exitInterviewRepository = exitInterviewRepository;
        _offboardingRepository = offboardingRepository;
        _auditLogService = auditLogService;
        _metrics = metrics;
    }

    /// <summary>
    /// Handles the command to record an exit interview.
    /// </summary>
    /// <param name="command">The record exit interview command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The unique identifier of the recorded exit interview.</returns>
    /// <exception cref="Exception">Thrown when offboarding workflow is not found or exit interview already exists.</exception>
    public async Task<Guid> HandleAsync(RecordExitInterviewCommand command, CancellationToken cancellationToken = default)
    {
        var offboarding = await _offboardingRepository.GetByEmployeeIdAsync(command.EmployeeId, cancellationToken);
        if (offboarding == null)
        {
            throw new Exception("Offboarding workflow not found for this employee");
        }

        var existing = await _exitInterviewRepository.GetByChecklistIdAsync(offboarding.Id, cancellationToken);
        if (existing != null)
        {
            throw new Exception("Exit interview already recorded for this offboarding workflow");
        }

        var exitInterview = new Domain.Entities.ExitInterview
        {
            Id = Guid.NewGuid(),
            OffboardingChecklistId = offboarding.Id,
            ConductedBy = command.ConductedBy,
            InterviewDate = command.InterviewDate,
            ReasonForLeaving = command.ReasonForLeaving,
            FeedbackOnManager = command.FeedbackOnManager,
            FeedbackOnTeam = command.FeedbackOnTeam,
            FeedbackOnCompany = command.FeedbackOnCompany,
            ImprovementSuggestions = command.ImprovementSuggestions,
            WouldRecommendCompany = command.WouldRecommendCompany,
            CreatedDate = DateTime.UtcNow
        };

        await _exitInterviewRepository.AddAsync(exitInterview, cancellationToken);

        _metrics.RecordExitInterviewRecorded();
        await _auditLogService.LogAsync("ExitInterview", exitInterview.Id, "Recorded", command.UserId, null, exitInterview, cancellationToken);

        return exitInterview.Id;
    }
}
