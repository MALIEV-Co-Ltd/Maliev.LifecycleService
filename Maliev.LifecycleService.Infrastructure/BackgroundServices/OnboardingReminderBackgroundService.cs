using Maliev.LifecycleService.Application.Interfaces;
using Maliev.MessagingContracts.Generated;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Maliev.LifecycleService.Infrastructure.BackgroundServices;

/// <summary>
/// Background service that checks for overdue and due-soon onboarding tasks and publishes events.
/// </summary>
public class OnboardingReminderBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OnboardingReminderBackgroundService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="OnboardingReminderBackgroundService"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="logger">The logger.</param>
    public OnboardingReminderBackgroundService(IServiceProvider serviceProvider, ILogger<OnboardingReminderBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("OnboardingReminderBackgroundService running at: {time}", DateTimeOffset.Now);

            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IOnboardingRepository>();
                var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
                var metrics = scope.ServiceProvider.GetRequiredService<ILifecycleMetrics>();

                var pendingChecklists = await repository.GetPendingAsync(0, 1000, stoppingToken);

                foreach (var checklist in pendingChecklists)
                {
                    var fullChecklist = await repository.GetByIdWithItemsAsync(checklist.Id, stoppingToken);
                    if (fullChecklist == null) continue;

                    foreach (var item in fullChecklist.Items.Where(i => !i.IsCompleted))
                    {
                        var dueDate = fullChecklist.StartDate.AddDays(item.DaysDue);
                        var now = DateTime.UtcNow;

                        // Overdue: 3+ days overdue
                        if (now > dueDate.AddDays(3))
                        {
                            _logger.LogWarning("Task {ItemId} is overdue since {DueDate}", item.Id, dueDate);
                            metrics.RecordTaskOverdue();
                            await eventPublisher.PublishAsync(new OnboardingItemOverdueEvent(
                                Guid.NewGuid(), // MessageId
                                nameof(OnboardingItemOverdueEvent), // MessageName
                                MessageType.Event, // MessageType (replace with correct value if needed)
                                "1.0", // MessageVersion
                                "OnboardingReminderBackgroundService", // PublishedBy
                                Array.Empty<string>(), // ConsumedBy (empty list or provide as needed)
                                Guid.NewGuid(), // CorrelationId (replace with actual correlation if available)
                                null, // CausationId (replace if available)
                                DateTimeOffset.UtcNow, // OccurredAtUtc
                                true, // IsPublic (set as needed)
                                new OnboardingItemOverdueEventPayload
                                {
                                    ItemId = item.Id,
                                    EmployeeId = checklist.EmployeeId,
                                    ItemTitle = item.Title,
                                    DueDate = dueDate
                                }
                            ), stoppingToken);
                        }
                        // Due soon: within 1 day
                        else if (now > dueDate.AddDays(-1) && now < dueDate)
                        {
                            _logger.LogInformation("Task {ItemId} is due soon: {DueDate}", item.Id, dueDate);
                        }
                    }
                }
            }

            // Run once a day (simplified for MVP: every 24 hours)
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}