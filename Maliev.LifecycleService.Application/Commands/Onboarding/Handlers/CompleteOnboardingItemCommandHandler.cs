using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Enums;
using Maliev.LifecycleService.Domain.Exceptions;
using Maliev.MessagingContracts.Generated;

namespace Maliev.LifecycleService.Application.Commands.Onboarding.Handlers;

/// <summary>
/// Handles the completion of an onboarding item.
/// </summary>
public class CompleteOnboardingItemCommandHandler
{
    private readonly IOnboardingRepository _repository;
    private readonly IAuditLogService _auditLogService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILifecycleMetrics _metrics;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompleteOnboardingItemCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The onboarding repository.</param>
    /// <param name="auditLogService">The audit logging service.</param>
    /// <param name="eventPublisher">The event publisher.</param>
    /// <param name="metrics">The business metrics service.</param>
    public CompleteOnboardingItemCommandHandler(
        IOnboardingRepository repository,
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
    /// Handles the completion of an onboarding item.
    /// </summary>
    /// <param name="command">The complete onboarding item command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown when the onboarding item is not found.</exception>
    /// <exception cref="ItemAlreadyCompletedException">Thrown when the item is already completed.</exception>
    public async Task HandleAsync(CompleteOnboardingItemCommand command, CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetItemByIdAsync(command.ItemId, cancellationToken);
        if (item == null)
        {
            throw new Exception("Onboarding item not found");
        }

        if (item.IsCompleted)
        {
            throw new ItemAlreadyCompletedException(command.ItemId);
        }

        // Capture a snapshot of the state before modification
        var beforeState = new { item.IsCompleted, item.CompletedDate, item.CompletedBy, item.Notes };

        item.IsCompleted = true;
        item.CompletedDate = DateTime.UtcNow;
        item.CompletedBy = command.UserId;
        item.Notes = command.Notes;

        var checklist = item.Checklist;
        checklist.CompletedItems++;

        if (checklist.Status == OnboardingStatus.NotStarted)
        {
            checklist.Status = OnboardingStatus.InProgress;
        }

        if (checklist.Items != null && checklist.TotalItems > 0 && checklist.CompletedItems >= checklist.TotalItems)
        {
            checklist.Status = OnboardingStatus.Completed;
            checklist.CompletedDate = DateTime.UtcNow;

            var duration = (checklist.CompletedDate.Value - checklist.CreatedDate).TotalDays;
            _metrics.RecordOnboardingDuration(duration);
            _metrics.RecordOnboardingCompleted();
            await _eventPublisher.PublishAsync(
                new OnboardingCompletedEvent(
                    MessageId: Guid.NewGuid(),
                    MessageName: nameof(OnboardingCompletedEvent),
                    MessageType: MessageType.Event,
                    MessageVersion: "1.0",
                    PublishedBy: "LifecycleService",
                    ConsumedBy: Array.Empty<string>(),
                    CorrelationId: Guid.NewGuid(),
                    CausationId: null,
                    OccurredAtUtc: DateTimeOffset.UtcNow,
                    IsPublic: false,
                    Payload: new OnboardingCompletedEventPayload(
                        ChecklistId: checklist.Id,
                        EmployeeId: checklist.EmployeeId,
                        CompletedDate: checklist.CompletedDate.Value
                    )
                ),
                cancellationToken
            );
        }

        await _repository.UpdateAsync(checklist, cancellationToken);

        // Publish LifecycleTaskCompletedEvent
        await _eventPublisher.PublishAsync(new Maliev.MessagingContracts.Generated.LifecycleTaskCompletedEvent(
            MessageId: Guid.NewGuid(),
            MessageName: nameof(Maliev.MessagingContracts.Generated.LifecycleTaskCompletedEvent),
            MessageType: Maliev.MessagingContracts.Generated.MessageType.Event,
            MessageVersion: "1.0",
            PublishedBy: "LifecycleService",
            ConsumedBy: Array.Empty<string>(),
            CorrelationId: Guid.NewGuid(),
            CausationId: null,
            OccurredAtUtc: DateTimeOffset.UtcNow,
            IsPublic: false,
            Payload: new Maliev.MessagingContracts.Generated.LifecycleTaskCompletedEventPayload(
                TaskId: item.Id,
                EmployeeId: checklist.EmployeeId,
                CompletedDate: item.CompletedDate.Value
            )
        ), cancellationToken);

        await _auditLogService.LogAsync(
            "OnboardingItem",
            item.Id,
            "Completed",
            command.UserId,
            beforeState,
            item,
            cancellationToken);
    }
}