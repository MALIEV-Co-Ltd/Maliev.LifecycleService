using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;
using Maliev.LifecycleService.Domain.Enums;
using Maliev.MessagingContracts;
using Maliev.MessagingContracts.Contracts.Lifecycle;

namespace Maliev.LifecycleService.Application.Commands.Offboarding.Handlers;

/// <summary>
/// Handles the initiation of an offboarding workflow.
/// </summary>
public class StartOffboardingCommandHandler
{
    private readonly IOffboardingRepository _repository;
    private readonly IAuditLogService _auditLogService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILifecycleMetrics _metrics;

    /// <summary>
    /// Initializes a new instance of the <see cref="StartOffboardingCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The offboarding repository.</param>
    /// <param name="auditLogService">The audit logging service.</param>
    /// <param name="eventPublisher">The event publisher.</param>
    /// <param name="metrics">The business metrics service.</param>
    public StartOffboardingCommandHandler(
        IOffboardingRepository repository,
        IAuditLogService auditLogService,
        IEventPublisher eventPublisher,
        ILifecycleMetrics metrics)
    {
        _repository = repository;
        _auditLogService = auditLogService;
        _eventPublisher = eventPublisher;
        _metrics = metrics;
    }

    /// <summary>
    /// Handles the starting of an offboarding workflow.
    /// </summary>
    /// <param name="command">The start offboarding command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The unique identifier of the created offboarding checklist.</returns>
    /// <exception cref="Exception">Thrown when offboarding has already started for the employee.</exception>
    public async Task<Guid> HandleAsync(StartOffboardingCommand command, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByEmployeeIdAsync(command.EmployeeId, cancellationToken);
        if (existing != null)
        {
            throw new Exception("Offboarding already started for this employee");
        }

        var checklist = new OffboardingChecklist
        {
            Id = Guid.NewGuid(),
            EmployeeId = command.EmployeeId,
            TerminationDate = command.TerminationDate,
            TerminationReason = command.TerminationReason,
            EligibleForRehire = command.EligibleForRehire,
            Status = OffboardingStatus.NotStarted,
            PaycheckReleaseBlocked = true,
            CreatedDate = DateTime.UtcNow
        };

        // Add default offboarding tasks
        checklist.Tasks = new List<OffboardingTask>
        {
            new() { Title = "Revoke Network Access", Category = TaskCategory.ITAccess, IsPaycheckBlocker = true, SortOrder = 1 },
            new() { Title = "Collect Company Equipment", Category = TaskCategory.EquipmentReturn, IsPaycheckBlocker = true, SortOrder = 2 },
            new() { Title = "Conduct Exit Interview", Category = TaskCategory.Documentation, IsPaycheckBlocker = false, SortOrder = 3 },
            new() { Title = "Final Payroll Processing", Category = TaskCategory.Financial, IsPaycheckBlocker = true, SortOrder = 4 }
        };

        await _repository.AddAsync(checklist, cancellationToken);

        _metrics.RecordOffboardingStarted();

        // Construct OffboardingStartedEvent with all required parameters
        var offboardingStartedEvent = new OffboardingStartedEvent(
            MessageId: Guid.NewGuid(),
            MessageName: nameof(OffboardingStartedEvent),
            MessageType: MessageType.Event,
            MessageVersion: "1.0",
            PublishedBy: "LifecycleService",
            ConsumedBy: Array.Empty<string>(),
            CorrelationId: checklist.Id,
            CausationId: null,
            OccurredAtUtc: DateTimeOffset.UtcNow,
            IsPublic: true,
            Payload: new OffboardingStartedEventPayload(
                ChecklistId: checklist.Id,
                EmployeeId: checklist.EmployeeId,
                TerminationDate: checklist.TerminationDate,
                TerminationReason: checklist.TerminationReason
            )
        );

        await _eventPublisher.PublishAsync(offboardingStartedEvent, cancellationToken);

        await _auditLogService.LogAsync("OffboardingChecklist", checklist.Id, "Started", command.UserId, null, checklist, cancellationToken);

        return checklist.Id;
    }
}
