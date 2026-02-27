using Maliev.LifecycleService.Domain.Commands;
using Maliev.MessagingContracts;
using Maliev.MessagingContracts.Contracts.Lifecycle;
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

        var accessRevokedEvent = new AccessRevokedEvent(
            MessageId: Guid.NewGuid(),
            MessageName: nameof(AccessRevokedEvent),
            MessageType: MessageType.Event,
            MessageVersion: "1.0.0",
            PublishedBy: "LifecycleService",
            ConsumedBy: new List<string> { "IAMService", "NotificationService" },
            CorrelationId: request.CorrelationId,
            CausationId: null,
            OccurredAtUtc: DateTimeOffset.UtcNow,
            IsPublic: false,
            Payload: new AccessRevokedEventPayload(
                EmployeeId: request.EmployeeId,
                RevokedDate: DateTimeOffset.UtcNow
            )
        );

        await _publishEndpoint.Publish(accessRevokedEvent, cancellationToken);

        return true;
    }
}
