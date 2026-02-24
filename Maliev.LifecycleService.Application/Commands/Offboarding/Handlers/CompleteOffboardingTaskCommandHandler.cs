using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Application.Services;
using Maliev.LifecycleService.Domain.Enums;
using Maliev.MessagingContracts.Generated;
using Maliev.MessagingContracts.Contracts.Lifecycle;

namespace Maliev.LifecycleService.Application.Commands.Offboarding.Handlers;

/// <summary>
/// Handles the completion of an offboarding task.
/// </summary>
public class CompleteOffboardingTaskCommandHandler
{
    private readonly IOffboardingRepository _repository;
    private readonly IPaycheckBlockingService _paycheckBlockingService;
    private readonly IAuditLogService _auditLogService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILifecycleMetrics _metrics;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompleteOffboardingTaskCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The offboarding repository.</param>
    /// <param name="paycheckBlockingService">The paycheck blocking calculation service.</param>
    /// <param name="auditLogService">The audit logging service.</param>
    /// <param name="eventPublisher">The event publisher.</param>
    /// <param name="metrics">The business metrics service.</param>
    public CompleteOffboardingTaskCommandHandler(
        IOffboardingRepository repository,
        IPaycheckBlockingService paycheckBlockingService,
        IAuditLogService auditLogService,
        IEventPublisher eventPublisher,
        ILifecycleMetrics metrics)
    {
        _repository = repository;
        _paycheckBlockingService = paycheckBlockingService;
        _auditLogService = auditLogService;
        _eventPublisher = eventPublisher;
        _metrics = metrics;
    }

    /// <summary>
    /// Handles the completion of an offboarding task.
    /// </summary>
    /// <param name="command">The complete offboarding task command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown when the offboarding task is not found.</exception>
    public async Task HandleAsync(CompleteOffboardingTaskCommand command, CancellationToken cancellationToken = default)
    {
        var task = await _repository.GetTaskByIdAsync(command.TaskId, cancellationToken);
        if (task == null)
        {
            throw new Exception("Offboarding task not found");
        }

        if (task.IsCompleted) return;

        // Capture a snapshot of the state before modification to avoid reference update in audit log
        var beforeState = new { task.IsCompleted, task.CompletedDate, task.CompletedBy, task.Notes };

        task.IsCompleted = true;
        task.CompletedDate = DateTime.UtcNow;
        task.CompletedBy = command.UserId;
        task.Notes = command.Notes;

        var checklist = task.Checklist;

        if (checklist.Status == OffboardingStatus.NotStarted)
        {
            checklist.Status = OffboardingStatus.InProgress;
        }

        // Update paycheck blocking flag
        checklist.PaycheckReleaseBlocked = _paycheckBlockingService.CalculatePaycheckBlocked(checklist);

        // Check if all tasks are completed - ensure Tasks collection is loaded and not empty
        if (checklist.Tasks != null && checklist.Tasks.Any() && checklist.Tasks.All(t => t.IsCompleted))
        {
            checklist.Status = OffboardingStatus.Completed;
            checklist.CompletedDate = DateTime.UtcNow;

            var duration = (checklist.CompletedDate.Value - checklist.CreatedDate).TotalDays;
            _metrics.RecordOffboardingDuration(duration);
            _metrics.RecordOffboardingCompleted();
            await _eventPublisher.PublishAsync(
                new OffboardingCompletedEvent(
                    MessageId: Guid.NewGuid(),
                    MessageName: nameof(OffboardingCompletedEvent),
                    MessageType: MessageType.Event,
                    MessageVersion: "1.0",
                    PublishedBy: "LifecycleService",
                    ConsumedBy: Array.Empty<string>(),
                    CorrelationId: Guid.NewGuid(),
                    CausationId: null,
                    OccurredAtUtc: DateTimeOffset.UtcNow,
                    IsPublic: false,
                    Payload: new OffboardingCompletedEventPayload(
                        ChecklistId: checklist.Id,
                        EmployeeId: checklist.EmployeeId,
                        CompletedDate: checklist.CompletedDate.Value
                    )
                ),
                cancellationToken
            );

            // Trigger access revocation if not already triggered by a specific task
            await _eventPublisher.PublishAsync(
                new AccessRevocationRequiredEvent(
                    MessageId: Guid.NewGuid(),
                    MessageName: nameof(AccessRevocationRequiredEvent),
                    MessageType: MessageType.Event,
                    MessageVersion: "1.0",
                    PublishedBy: "LifecycleService",
                    ConsumedBy: Array.Empty<string>(),
                    CorrelationId: Guid.NewGuid(),
                    CausationId: null,
                    OccurredAtUtc: DateTimeOffset.UtcNow,
                    IsPublic: false,
                    Payload: new AccessRevocationRequiredEventPayload(
                        EmployeeId: checklist.EmployeeId,
                        EffectiveDate: DateTimeOffset.UtcNow,
                        Reason: "Offboarding completed"
                    )
                ),
                cancellationToken
            );
        }

        await _repository.UpdateAsync(checklist, cancellationToken);

        // Publish LifecycleTaskCompletedEvent
        await _eventPublisher.PublishAsync(new LifecycleTaskCompletedEvent(
            MessageId: Guid.NewGuid(),
            MessageName: nameof(LifecycleTaskCompletedEvent),
            MessageType: MessageType.Event,
            MessageVersion: "1.0",
            PublishedBy: "LifecycleService",
            ConsumedBy: Array.Empty<string>(),
            CorrelationId: Guid.NewGuid(),
            CausationId: null,
            OccurredAtUtc: DateTimeOffset.UtcNow,
            IsPublic: false,
            Payload: new LifecycleTaskCompletedEventPayload(
                TaskId: task.Id,
                EmployeeId: checklist.EmployeeId,
                CompletedDate: task.CompletedDate.Value
            )
        ), cancellationToken);

        await _auditLogService.LogAsync("OffboardingTask", task.Id, "Completed", command.UserId, beforeState, task, cancellationToken);
    }
}