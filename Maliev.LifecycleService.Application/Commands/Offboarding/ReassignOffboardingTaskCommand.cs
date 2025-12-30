namespace Maliev.LifecycleService.Application.Commands.Offboarding;

/// <summary>
/// Command to reassign a specific offboarding task to another user or role.
/// </summary>
/// <param name="TaskId">The unique identifier of the offboarding task.</param>
/// <param name="AssignedTo">The identifier of the new assignee.</param>
/// <param name="UserId">The identifier of the user performing the reassignment.</param>
public record ReassignOffboardingTaskCommand(Guid TaskId, Guid AssignedTo, Guid UserId);