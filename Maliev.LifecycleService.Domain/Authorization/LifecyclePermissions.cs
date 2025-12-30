namespace Maliev.LifecycleService.Domain.Authorization;

/// <summary>
/// Defines permission constants for lifecycle service operations.
/// Follows format: service.resource.action
/// </summary>
public static class LifecyclePermissions
{
    /// <summary>
    /// Permission to manage onboarding/offboarding workflows, complete tasks, and reassign items.
    /// </summary>
    public const string Manage = "lifecycle.workflows.manage";

    /// <summary>
    /// Permission to manage templates and access all lifecycle data (administrative functions).
    /// </summary>
    public const string Admin = "lifecycle.admin.manage";
}
