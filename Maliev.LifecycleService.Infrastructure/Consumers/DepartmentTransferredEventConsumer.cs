using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;
using Maliev.MessagingContracts.Contracts.Employee;
using Maliev.MessagingContracts.Contracts.Lifecycle;
using Maliev.MessagingContracts.Generated;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Maliev.LifecycleService.Infrastructure.Consumers;

/// <summary>
/// Consumer for DepartmentTransferredEvent.
/// Triggers onboarding checklist creation if the employee was previously unassigned.
/// </summary>
public class DepartmentTransferredEventConsumer : IConsumer<DepartmentTransferredEvent>
{
    private readonly IOnboardingRepository _onboardingRepository;
    private readonly ITemplateRepository _templateRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILifecycleMetrics _metrics;
    private readonly ILogger<DepartmentTransferredEventConsumer> _logger;

    public DepartmentTransferredEventConsumer(
        IOnboardingRepository onboardingRepository,
        ITemplateRepository templateRepository,
        IEventPublisher eventPublisher,
        ILifecycleMetrics metrics,
        ILogger<DepartmentTransferredEventConsumer> logger)
    {
        _onboardingRepository = onboardingRepository;
        _templateRepository = templateRepository;
        _eventPublisher = eventPublisher;
        _metrics = metrics;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DepartmentTransferredEvent> context)
    {
        var payload = context.Message.Payload;
        _logger.LogInformation("Processing DepartmentTransferred event for Employee {EmployeeId} to Department {NewDepartmentId}",
            payload.EmployeeId, payload.NewDepartmentId);

        // Check if onboarding checklist already exists
        var existing = await _onboardingRepository.GetByEmployeeIdAsync(payload.EmployeeId);
        if (existing != null)
        {
            _logger.LogDebug("Employee {EmployeeId} already has an onboarding checklist. Skipping creation.", payload.EmployeeId);
            return;
        }

        // Only trigger if transferred to a valid department
        if (payload.NewDepartmentId == Guid.Empty)
        {
            return;
        }

        // Try to find a template for the new department
        var template = await _templateRepository.GetByDepartmentIdAsync(payload.NewDepartmentId);
        if (template == null)
        {
            _logger.LogWarning("No onboarding template found for the new department {DepartmentId}. Cannot auto-create checklist.",
                payload.NewDepartmentId);
            return;
        }

        // Create the checklist
        var checklist = new OnboardingChecklist
        {
            Id = Guid.NewGuid(),
            EmployeeId = payload.EmployeeId,
            StartDate = payload.EffectiveDate.UtcDateTime,
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

        // Publish internal event
        await _eventPublisher.PublishAsync(new OnboardingStartedEvent(
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
        ));

        _logger.LogInformation("Successfully initialized onboarding checklist {ChecklistId} for employee {EmployeeId} after department assignment.",
            checklist.Id, payload.EmployeeId);
    }
}
