using Maliev.MessagingContracts.Generated;
using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;
using DomainEvents = Maliev.LifecycleService.Domain.Events;
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
            CreatedDate = DateTime.UtcNow
        };

        await _offboardingRepository.AddAsync(checklist);

        _metrics.RecordOffboardingStarted();
        await _eventPublisher.PublishAsync(new DomainEvents.OffboardingStartedEvent(checklist.Id, checklist.EmployeeId, checklist.TerminationDate));

        _logger.LogInformation("Successfully created offboarding checklist {ChecklistId} for employee {EmployeeId}", checklist.Id, payload.EmployeeId);
    }
}
