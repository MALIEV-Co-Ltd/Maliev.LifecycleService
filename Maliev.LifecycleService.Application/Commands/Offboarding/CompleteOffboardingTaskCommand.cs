namespace Maliev.LifecycleService.Application.Commands.Offboarding;

/// <summary>
/// Command to complete a specific offboarding task.
/// </summary>
/// <param name="TaskId">The unique identifier of the offboarding task.</param>
/// <param name="UserId">The identifier of the user completing the task.</param>
/// <param name="Notes">Optional notes regarding the task completion.</param>
public record CompleteOffboardingTaskCommand(Guid TaskId, Guid UserId, string? Notes = null);
