using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Commands;
using Maliev.LifecycleService.Domain.IntegrationEvents;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Maliev.LifecycleService.Application.Commands.Offboarding.Handlers;

/// <summary>
/// Handler for RevokeAccessCommand (Saga step).
/// </summary>
public class RevokeAccessCommandHandler : IRequestHandler<RevokeAccessCommand, bool>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<RevokeAccessCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RevokeAccessCommandHandler"/> class.
    /// </summary>
    /// <param name="publishEndpoint">The message bus publish endpoint.</param>
    /// <param name="logger">The logger.</param>
    public RevokeAccessCommandHandler(
        IPublishEndpoint publishEndpoint,
        ILogger<RevokeAccessCommandHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<bool> Handle(RevokeAccessCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Revoking access for employee {EmployeeId} (Correlation: {CorrelationId})",
            request.EmployeeId, request.CorrelationId);

        // Logic to revoke system access via IAM or other mechanisms
        // ...

        await _publishEndpoint.Publish(new AccessRevokedEvent(request.EmployeeId, request.CorrelationId), cancellationToken);

        return true;
    }
}