using Maliev.LifecycleService.Application.Commands.Handlers;
using Maliev.LifecycleService.Domain.Commands;
using MassTransit;

namespace Maliev.LifecycleService.Infrastructure.Consumers;

/// <summary>
/// Consumes <see cref="UndoRevokeAccessCommand"/> to revert access revocation.
/// </summary>
public class UndoRevokeAccessConsumer : IConsumer<UndoRevokeAccessCommand>
{
    private readonly UndoRevokeAccessCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="UndoRevokeAccessConsumer"/> class.
    /// </summary>
    /// <param name="handler">The command handler.</param>
    public UndoRevokeAccessConsumer(UndoRevokeAccessCommandHandler handler)
    {
        _handler = handler;
    }

    /// <inheritdoc/>
    public async Task Consume(ConsumeContext<UndoRevokeAccessCommand> context)
    {
        await _handler.HandleAsync(context.Message, context.CancellationToken);
    }
}
