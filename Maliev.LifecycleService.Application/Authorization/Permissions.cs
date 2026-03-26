namespace Maliev.LifecycleService.Application.Authorization;

/// <summary>
/// Defines the permissions for the Lifecycle Service.
/// </summary>
public static class LifecyclePermissions
{
    public const string MilestoneCreate = "lifecycle.milestones.create";
    public const string MilestoneRead = "lifecycle.milestones.read";
    public const string MilestoneUpdate = "lifecycle.milestones.update";
    public const string MilestoneClose = "lifecycle.milestones.close";

    public const string PhaseRead = "lifecycle.phases.read";
    public const string PhaseManage = "lifecycle.phases.manage";

    public const string TaskCreate = "lifecycle.tasks.create";
    public const string TaskRead = "lifecycle.tasks.read";
    public const string TaskUpdate = "lifecycle.tasks.update";
    public const string TaskComplete = "lifecycle.tasks.complete";

    public const string ReportRead = "lifecycle.reports.read";

    public static readonly IReadOnlyDictionary<string, string> AllWithDescriptions = new Dictionary<string, string>
    {
        { MilestoneCreate, "Create lifecycle milestones" },
        { MilestoneRead, "Read lifecycle milestones" },
        { MilestoneUpdate, "Update lifecycle milestones" },
        { MilestoneClose, "Close lifecycle milestones" },
        { PhaseRead, "Read lifecycle phases" },
        { PhaseManage, "Manage lifecycle phases" },
        { TaskCreate, "Create lifecycle tasks" },
        { TaskRead, "Read lifecycle tasks" },
        { TaskUpdate, "Update lifecycle tasks" },
        { TaskComplete, "Complete lifecycle tasks" },
        { ReportRead, "Read lifecycle reports" },
    };

    public static string[] All => AllWithDescriptions.Keys.ToArray();
}
