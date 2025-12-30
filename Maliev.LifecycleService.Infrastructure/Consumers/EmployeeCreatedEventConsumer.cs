using Maliev.EmployeeService.Domain.IntegrationEvents;
using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;
using Maliev.LifecycleService.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Maliev.LifecycleService.Infrastructure.Consumers;

/// <summary>
/// Consumer for EmployeeCreatedIntegrationEvent that automatically initializes onboarding checklists.
/// </summary>
public class EmployeeCreatedEventConsumer : IConsumer<EmployeeCreatedIntegrationEvent>
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
    public async Task Consume(ConsumeContext<EmployeeCreatedIntegrationEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing EmployeeCreatedIntegrationEvent for EmployeeId: {EmployeeId}", message.EmployeeId);

        var existing = await _onboardingRepository.GetByEmployeeIdAsync(message.EmployeeId);
        if (existing != null)
        {
            _logger.LogWarning("Onboarding checklist already exists for employee {EmployeeId}", message.EmployeeId);
            return;
        }

        var template = await _templateRepository.GetByDepartmentIdAsync(message.DepartmentId);
        if (template == null)
        {
            _logger.LogWarning("No onboarding template found for department {DepartmentId}. Skipping auto-creation.", message.DepartmentId);
            return;
        }

        var checklist = new OnboardingChecklist
        {
            Id = Guid.NewGuid(),
            EmployeeId = message.EmployeeId,
            StartDate = message.HireDate,
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
        await _eventPublisher.PublishAsync(new OnboardingStartedEvent(checklist.Id, checklist.EmployeeId, checklist.StartDate));

        _logger.LogInformation("Successfully created onboarding checklist {ChecklistId} for employee {EmployeeId}", checklist.Id, message.EmployeeId);
    }
}
