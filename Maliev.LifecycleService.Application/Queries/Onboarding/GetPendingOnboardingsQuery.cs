namespace Maliev.LifecycleService.Application.Queries.Onboarding;

/// <summary>
/// Query to retrieve a paginated list of pending onboarding checklists.
/// </summary>
/// <param name="Offset">The number of items to skip.</param>
/// <param name="Limit">The maximum number of items to return.</param>
public record GetPendingOnboardingsQuery(int Offset = 0, int Limit = 50);