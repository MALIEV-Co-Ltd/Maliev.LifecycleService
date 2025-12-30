using Maliev.EmployeeService.Domain.IntegrationEvents;
using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;
using Maliev.LifecycleService.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Maliev.LifecycleService.Infrastructure.Consumers;

/// <summary>
/// Consumer for EmployeeTerminatedIntegrationEvent that automatically initializes offboarding checklists.
/// </summary>
public class EmployeeTerminatedEventConsumer : IConsumer<EmployeeTerminatedIntegrationEvent>
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
    public async Task Consume(ConsumeContext<EmployeeTerminatedIntegrationEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing EmployeeTerminatedIntegrationEvent for EmployeeId: {EmployeeId}", message.EmployeeId);

        var existing = await _offboardingRepository.GetByEmployeeIdAsync(message.EmployeeId);
        if (existing != null)
        {
            _logger.LogWarning("Offboarding checklist already exists for employee {EmployeeId}", message.EmployeeId);
            return;
        }

        var checklist = new OffboardingChecklist
        {
            Id = Guid.NewGuid(),
            EmployeeId = message.EmployeeId,
            TerminationDate = message.TerminationDate,
            CreatedDate = DateTime.UtcNow
        };

        await _offboardingRepository.AddAsync(checklist);

        _metrics.RecordOffboardingStarted();
        await _eventPublisher.PublishAsync(new OffboardingStartedEvent(checklist.Id, checklist.EmployeeId, checklist.TerminationDate));

        _logger.LogInformation("Successfully created offboarding checklist {ChecklistId} for employee {EmployeeId}", checklist.Id, message.EmployeeId);
    }
}
