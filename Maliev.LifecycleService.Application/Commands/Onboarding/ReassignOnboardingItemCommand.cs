namespace Maliev.LifecycleService.Application.Commands.Onboarding;

/// <summary>
/// Command to reassign a specific onboarding item to another user or role.
/// </summary>
/// <param name="ItemId">The unique identifier of the onboarding item.</param>
/// <param name="AssignedTo">The identifier of the new assignee.</param>
/// <param name="UserId">The identifier of the user performing the reassignment.</param>
public record ReassignOnboardingItemCommand(Guid ItemId, Guid AssignedTo, Guid UserId);
