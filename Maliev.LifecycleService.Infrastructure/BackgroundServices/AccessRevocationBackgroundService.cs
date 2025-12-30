using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Enums;
using Maliev.LifecycleService.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Maliev.LifecycleService.Infrastructure.BackgroundServices;

/// <summary>
/// Background service that periodically checks for pending offboarding workflows and logs potential revocation needs.
/// </summary>
public class AccessRevocationBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AccessRevocationBackgroundService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessRevocationBackgroundService"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="logger">The logger.</param>
    public AccessRevocationBackgroundService(IServiceProvider serviceProvider, ILogger<AccessRevocationBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("AccessRevocationBackgroundService running at: {time}", DateTimeOffset.Now);

            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IOffboardingRepository>();
                var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

                // In a real scenario, we might want to track if revocation was already sent.
                var pending = await repository.GetPendingAsync(0, 100, stoppingToken);
                
                foreach (var checklist in pending)
                {
                    if (checklist.TerminationDate.Date <= DateTime.UtcNow.Date)
                    {
                        _logger.LogInformation("Found active offboarding for employee {EmployeeId} with termination date {Date}", 
                            checklist.EmployeeId, checklist.TerminationDate);
                    }
                }
            }

            // Run once an hour
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}