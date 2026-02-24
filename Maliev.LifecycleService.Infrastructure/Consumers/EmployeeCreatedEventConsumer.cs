using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;
using Maliev.MessagingContracts.Generated;
using Maliev.MessagingContracts.Contracts.Employee;
using Maliev.MessagingContracts.Contracts.Lifecycle;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Maliev.LifecycleService.Infrastructure.Consumers;

/// <summary>
/// Consumer for EmployeeCreatedEvent that automatically initializes onboarding checklists.
/// </summary>
public class EmployeeCreatedEventConsumer : IConsumer<EmployeeCreatedEvent>
{
    private readonly IOnboardingRepository _onboardingRepository;
    private readonly ITemplateRepository _templateRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILifecycleMetrics _metrics;
    private readonly ILogger<EmployeeCreatedEventConsumer> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeCreatedEventConsumer"/> class.
    /// </summary>
    /// <param name="onboardingRepository">The onboarding repository.</param>
    /// <param name="templateRepository">The template repository.</param>
    /// <param name="eventPublisher">The event publisher.</param>
    /// <param name="metrics">The lifecycle metrics.</param>
    /// <param name="logger">The logger.</param>
    public EmployeeCreatedEventConsumer(
        IOnboardingRepository onboardingRepository,
        ITemplateRepository templateRepository,
        IEventPublisher eventPublisher,
        ILifecycleMetrics metrics,
        ILogger<EmployeeCreatedEventConsumer> logger)
    {
        _onboardingRepository = onboardingRepository;
        _templateRepository = templateRepository;
        _eventPublisher = eventPublisher;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task Consume(ConsumeContext<EmployeeCreatedEvent> context)
    {
        var message = context.Message;
        var payload = message.Payload;
        _logger.LogInformation("Processing EmployeeCreatedEvent for EmployeeId: {EmployeeId}", payload.EmployeeId);

        var existing = await _onboardingRepository.GetByEmployeeIdAsync(payload.EmployeeId);
        if (existing != null)
        {
            _logger.LogWarning("Onboarding checklist already exists for employee {EmployeeId}", payload.EmployeeId);
            return;
        }

        var template = await _templateRepository.GetByDepartmentIdAsync(payload.DepartmentId);
        if (template == null)
        {
            if (payload.DepartmentId == Guid.Empty)
            {
                _logger.LogInformation("Employee {EmployeeId} is not assigned to a department. Skipping auto-onboarding creation.", payload.EmployeeId);
            }
            else
            {
                _logger.LogWarning("No onboarding template found for department {DepartmentId}. Skipping auto-creation.", payload.DepartmentId);
            }
            return;
        }


        var checklist = new OnboardingChecklist
        {
            Id = Guid.NewGuid(),
            EmployeeId = payload.EmployeeId,
            StartDate = payload.StartDate.UtcDateTime,
            TotalItems = template.Items.Count,
            CompletedItems = 0,
            CreatedDate = DateTime.UtcNow
        };

        foreach (var templateItem in template.Items.OrderBy(x => x.SortOrder))
        {
            checklist.Items.Add(new OnboardingItem
            {
                Id = Guid.NewGuid(),
                OnboardingChecklistId = checklist.Id,
                Title = templateItem.Title,
                Description = templateItem.Description,
                Category = templateItem.Category,
                DaysDue = templateItem.DaysDue,
                SortOrder = templateItem.SortOrder,
                CreatedDate = DateTime.UtcNow
            });
        }

        await _onboardingRepository.AddAsync(checklist);

        _metrics.RecordOnboardingStarted();
        await _eventPublisher.PublishAsync(
            new OnboardingStartedEvent(
                MessageId: Guid.NewGuid(),
                MessageName: nameof(OnboardingStartedEvent),
                MessageType: MessageType.Event,
                MessageVersion: "1.0",
                PublishedBy: "LifecycleService",
                ConsumedBy: Array.Empty<string>(),
                CorrelationId: context.CorrelationId ?? Guid.NewGuid(),
                CausationId: context.MessageId,
                OccurredAtUtc: DateTimeOffset.UtcNow,
                IsPublic: true,
                Payload: new OnboardingStartedEventPayload(
                    ChecklistId: checklist.Id,
                    EmployeeId: checklist.EmployeeId,
                    StartDate: checklist.StartDate,
                    TotalItems: checklist.TotalItems
                )
            )
        );

        _logger.LogInformation("Successfully created onboarding checklist {ChecklistId} for employee {EmployeeId}", checklist.Id, payload.EmployeeId);
    }
}
