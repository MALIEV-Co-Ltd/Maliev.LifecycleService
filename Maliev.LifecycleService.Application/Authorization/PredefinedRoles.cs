namespace Maliev.LifecycleService.Application.Authorization;

/// <summary>
/// Provides access to predefined roles for the Lifecycle Service.
/// </summary>
public static class LifecyclePredefinedRoles
{
    public const string Admin = "roles.lifecycle.admin";
    public const string Manager = "roles.lifecycle.manager";
    public const string Viewer = "roles.lifecycle.viewer";

    public static readonly IReadOnlyList<(string RoleId, string Description, string[] Permissions)> All = new List<(string, string, string[])>
    {
        (
            Admin,
            "Lifecycle Administrator with full access",
            new[]
            {
                LifecyclePermissions.MilestoneCreate,
                LifecyclePermissions.MilestoneRead,
                LifecyclePermissions.MilestoneUpdate,
                LifecyclePermissions.MilestoneClose,
                LifecyclePermissions.PhaseRead,
                LifecyclePermissions.PhaseManage,
                LifecyclePermissions.TaskCreate,
                LifecyclePermissions.TaskRead,
                LifecyclePermissions.TaskUpdate,
                LifecyclePermissions.TaskComplete,
                LifecyclePermissions.ReportRead,
            }
        ),
        (
            Manager,
            "Lifecycle Manager with milestone and task access",
            new[]
            {
                LifecyclePermissions.MilestoneRead,
                LifecyclePermissions.MilestoneUpdate,
                LifecyclePermissions.MilestoneClose,
                LifecyclePermissions.PhaseRead,
                LifecyclePermissions.TaskCreate,
                LifecyclePermissions.TaskRead,
                LifecyclePermissions.TaskUpdate,
                LifecyclePermissions.TaskComplete,
                LifecyclePermissions.ReportRead,
            }
        ),
        (
            Viewer,
            "Lifecycle Viewer with read-only access",
            new[]
            {
                LifecyclePermissions.MilestoneRead,
                LifecyclePermissions.PhaseRead,
                LifecyclePermissions.TaskRead,
                LifecyclePermissions.ReportRead,
            }
        ),
    };
}
