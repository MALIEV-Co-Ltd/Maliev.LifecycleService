namespace Maliev.LifecycleService.Application.Commands.Onboarding;

/// <summary>
/// Command to complete a specific onboarding item.
/// </summary>
/// <param name="ItemId">The unique identifier of the onboarding item.</param>
/// <param name="UserId">The identifier of the user completing the item.</param>
/// <param name="Notes">Optional notes regarding the item completion.</param>
public record CompleteOnboardingItemCommand(Guid ItemId, Guid UserId, string? Notes = null);