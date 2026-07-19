namespace Maliev.LifecycleService.Application.Queries.Offboarding;

/// <summary>
/// Query to retrieve a paginated list of pending offboarding workflows.
/// </summary>
/// <param name="Offset">The number of items to skip.</param>
/// <param name="Limit">The maximum number of items to return.</param>
public record GetPendingOffboardingsQuery(int Offset = 0, int Limit = 50);
