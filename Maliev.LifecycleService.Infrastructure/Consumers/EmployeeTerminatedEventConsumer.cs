using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Application.Commands.Offboarding.Handlers;
using Maliev.MessagingContracts.Generated;
using Maliev.MessagingContracts.Contracts.Employee;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Maliev.LifecycleService.Infrastructure.Consumers;

/// <summary>
/// Consumer for EmployeeTerminatedEvent that triggers offboarding workflows.
/// </summary>
public class EmployeeTerminatedEventConsumer : IConsumer<EmployeeTerminatedEvent>
{
    private readonly StartOffboardingCommandHandler _handler;
    private readonly ILogger<EmployeeTerminatedEventConsumer> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeTerminatedEventConsumer"/> class.
    /// </summary>
    public EmployeeTerminatedEventConsumer(StartOffboardingCommandHandler handler, ILogger<EmployeeTerminatedEventConsumer> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task Consume(ConsumeContext<EmployeeTerminatedEvent> context)
    {
        var payload = context.Message.Payload;
        _logger.LogInformation("Processing EmployeeTerminatedEvent for EmployeeId: {EmployeeId}", payload.EmployeeId);

        try
        {
            var command = new Maliev.LifecycleService.Application.Commands.Offboarding.StartOffboardingCommand(
                payload.EmployeeId,
                payload.TerminationDate.UtcDateTime,
                payload.TerminationReason ?? "No reason provided",
                payload.EligibleForRehire,
                Guid.Empty // System-auto
            );

            await _handler.HandleAsync(command);
            _logger.LogInformation("Successfully started offboarding for employee {EmployeeId}", payload.EmployeeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start offboarding for employee {EmployeeId}", payload.EmployeeId);
            // We don't re-throw here to avoid endless retries if it's a business logic error (e.g., already started)
        }
    }
}
