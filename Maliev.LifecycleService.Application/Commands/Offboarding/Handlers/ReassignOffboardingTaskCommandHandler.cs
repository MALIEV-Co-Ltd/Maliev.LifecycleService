using Maliev.LifecycleService.Application.Interfaces;

namespace Maliev.LifecycleService.Application.Commands.Offboarding.Handlers;

/// <summary>
/// Handles the reassignment of an offboarding task.
/// </summary>
public class ReassignOffboardingTaskCommandHandler
{
    private readonly IOffboardingRepository _repository;
    private readonly IAuditLogService _auditLogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReassignOffboardingTaskCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The offboarding repository.</param>
    /// <param name="auditLogService">The audit logging service.</param>
    public ReassignOffboardingTaskCommandHandler(IOffboardingRepository repository, IAuditLogService auditLogService)
    {
        _repository = repository;
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// Handles the reassignment of an offboarding task.
    /// </summary>
    /// <param name="command">The reassign offboarding task command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown when the offboarding task is not found.</exception>
    public async Task HandleAsync(ReassignOffboardingTaskCommand command, CancellationToken cancellationToken = default)
    {
        var task = await _repository.GetTaskByIdAsync(command.TaskId, cancellationToken);
        if (task == null)
        {
            throw new Exception("Offboarding task not found");
        }

        var beforeState = new { task.AssignedTo };

        task.AssignedTo = command.AssignedTo;

        await _repository.UpdateAsync(task.Checklist, cancellationToken);

        await _auditLogService.LogAsync("OffboardingTask", task.Id, "Reassigned", command.UserId, beforeState, new { task.AssignedTo }, cancellationToken);
    }
}
