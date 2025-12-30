using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;
using Maliev.LifecycleService.Domain.Enums;
using Maliev.LifecycleService.Domain.Events;

namespace Maliev.LifecycleService.Application.Commands.Onboarding.Handlers;

/// <summary>
/// Handles the manual initiation of an onboarding workflow.
/// </summary>
public class StartOnboardingCommandHandler
{
    private readonly IOnboardingRepository _onboardingRepository;
    private readonly ITemplateRepository _templateRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILifecycleMetrics _metrics;

    /// <summary>
    /// Initializes a new instance of the <see cref="StartOnboardingCommandHandler"/> class.
    /// </summary>
    /// <param name="onboardingRepository">The onboarding repository.</param>
    /// <param name="templateRepository">The template repository.</param>
    /// <param name="auditLogService">The audit logging service.</param>
    /// <param name="eventPublisher">The event publisher.</param>
    /// <param name="metrics">The business metrics service.</param>
    public StartOnboardingCommandHandler(
        IOnboardingRepository onboardingRepository,
        ITemplateRepository templateRepository,
        IAuditLogService auditLogService,
        IEventPublisher eventPublisher,
        ILifecycleMetrics metrics)
    {
        _onboardingRepository = onboardingRepository;
        _templateRepository = templateRepository;
        _auditLogService = auditLogService;
        _eventPublisher = eventPublisher;
        _metrics = metrics;
    }

    /// <summary>
    /// Handles the starting of an onboarding workflow.
    /// </summary>
    /// <param name="command">The start onboarding command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The unique identifier of the created onboarding checklist.</returns>
    /// <exception cref="Exception">Thrown when onboarding has already started for the employee or no template is found.</exception>
    public async Task<Guid> HandleAsync(StartOnboardingCommand command, CancellationToken cancellationToken = default)
    {
        // 1. Check if already started
        var existing = await _onboardingRepository.GetByEmployeeIdAsync(command.EmployeeId, cancellationToken);
        if (existing != null)
        {
            throw new Exception("Onboarding already started for this employee");
        }

        // 2. Get template
        OnboardingTemplate? template;
        if (command.TemplateId.HasValue)
        {
            template = await _templateRepository.GetByIdWithItemsAsync(command.TemplateId.Value, cancellationToken);
        }
        else
        {
            // Default active template
            template = await _templateRepository.GetByDepartmentIdAsync(null, cancellationToken);
        }

        if (template == null)
        {
            throw new Exception("No onboarding template found");
        }

        // 3. Create checklist
        var checklist = new OnboardingChecklist
        {
            Id = Guid.NewGuid(),
            EmployeeId = command.EmployeeId,
            StartDate = command.StartDate,
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

        await _onboardingRepository.AddAsync(checklist, cancellationToken);

        _metrics.RecordOnboardingStarted();
        await _eventPublisher.PublishAsync(new OnboardingStartedEvent(checklist.Id, checklist.EmployeeId, checklist.StartDate), cancellationToken);

        await _auditLogService.LogAsync("OnboardingChecklist", checklist.Id, "ManuallyStarted", command.UserId, null, checklist, cancellationToken);

        return checklist.Id;
    }
}