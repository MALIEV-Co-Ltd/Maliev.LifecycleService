namespace Maliev.LifecycleService.Domain.Authorization;

/// <summary>
/// Constants for Lifecycle Service permissions.
/// Follows GCP-style naming: {service}.{resource}.{action}
/// </summary>
public static class LifecyclePermissions
{
    /// <summary>Permission to manage onboarding/offboarding workflows.</summary>
    public const string Manage = "lifecycle.workflows.manage";

    /// <summary>Permission to manage templates and administrative functions.</summary>
    public const string Admin = "lifecycle.admin.manage";

    /// <summary>
    /// Collection of all permissions for easy registration.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, string> All = new Dictionary<string, string>
    {
        { Manage, "Manage onboarding/offboarding workflows" },
        { Admin, "Manage templates and administrative functions" }
    };
}