using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;
using Maliev.LifecycleService.Domain.Enums;
using Maliev.MessagingContracts.Generated;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Maliev.LifecycleService.Infrastructure.Consumers;

/// <summary>
/// Consumer for EmployeeTerminatedEvent that automatically initializes offboarding checklists.
/// </summary>
public class EmployeeTerminatedEventConsumer : IConsumer<EmployeeTerminatedEvent>
{
    private readonly IOffboardingRepository _offboardingRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILifecycleMetrics _metrics;
    private readonly ILogger<EmployeeTerminatedEventConsumer> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeTerminatedEventConsumer"/> class.
    /// </summary>
    /// <param name="offboardingRepository">The offboarding repository.</param>
    /// <param name="eventPublisher">The event publisher.</param>
    /// <param name="metrics">The lifecycle metrics.</param>
    /// <param name="logger">The logger.</param>
    public EmployeeTerminatedEventConsumer(
        IOffboardingRepository offboardingRepository,
        IEventPublisher eventPublisher,
        ILifecycleMetrics metrics,
        ILogger<EmployeeTerminatedEventConsumer> logger)
    {
        _offboardingRepository = offboardingRepository;
        _eventPublisher = eventPublisher;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task Consume(ConsumeContext<EmployeeTerminatedEvent> context)
    {
        var message = context.Message;
        var payload = message.Payload;
        _logger.LogInformation("Processing EmployeeTerminatedEvent for EmployeeId: {EmployeeId}", payload.EmployeeId);

        var existing = await _offboardingRepository.GetByEmployeeIdAsync(payload.EmployeeId);
        if (existing != null)
        {
            _logger.LogWarning("Offboarding checklist already exists for employee {EmployeeId}", payload.EmployeeId);
            return;
        }

        var checklist = new OffboardingChecklist
        {
            Id = Guid.NewGuid(),
            EmployeeId = payload.EmployeeId,
            TerminationDate = payload.TerminationDate.UtcDateTime,
            TerminationReason = payload.TerminationReason ?? "Terminated",
            EligibleForRehire = payload.EligibleForRehire,
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

        await _offboardingRepository.AddAsync(checklist);

        _metrics.RecordOffboardingStarted();
        await _eventPublisher.PublishAsync(
            new OffboardingStartedEvent
            {
                MessageId = Guid.NewGuid(),
                MessageName = nameof(OffboardingStartedEvent),
                MessageType = MessageType.Event,
                MessageVersion = "1.0",
                PublishedBy = "LifecycleService",
                ConsumedBy = Array.Empty<string>(),
                CorrelationId = checklist.Id,
                CausationId = null,
                OccurredAtUtc = DateTimeOffset.UtcNow,
                IsPublic = true,
                Payload = new OffboardingStartedEventPayload
                {
                    ChecklistId = checklist.Id,
                    EmployeeId = checklist.EmployeeId,
                    TerminationDate = checklist.TerminationDate
                }
            }
        );

        _logger.LogInformation("Successfully created offboarding checklist {ChecklistId} for employee {EmployeeId}", checklist.Id, payload.EmployeeId);
    }
}
