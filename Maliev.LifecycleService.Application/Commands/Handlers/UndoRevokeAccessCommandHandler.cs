using Maliev.LifecycleService.Domain.Commands;
using Microsoft.Extensions.Logging;

namespace Maliev.LifecycleService.Application.Commands.Handlers;

/// <summary>
/// Handler for UndoRevokeAccessCommand (Compensating Transaction).
/// </summary>
public class UndoRevokeAccessCommandHandler
{
    private readonly ILogger<UndoRevokeAccessCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UndoRevokeAccessCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public UndoRevokeAccessCommandHandler(ILogger<UndoRevokeAccessCommandHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the undo access revocation command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task HandleAsync(UndoRevokeAccessCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("UNDO: Restoring system access for employee {EmployeeId}", command.EmployeeId);

        // Logic to restore access or notify security

        await Task.CompletedTask;
    }
}