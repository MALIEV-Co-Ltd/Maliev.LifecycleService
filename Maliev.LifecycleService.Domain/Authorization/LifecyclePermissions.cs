namespace Maliev.LifecycleService.Domain.Authorization;

/// <summary>
/// Constants for Lifecycle Service permissions.
/// Follows GCP-style naming: {service}.{plural-resource}.{action}
/// </summary>
public static class LifecyclePermissions
{
    /// <summary>Permission to read onboarding checklists.</summary>
    public const string OnboardingsRead = "lifecycle.onboardings.read";

    /// <summary>Permission to create onboarding checklists.</summary>
    public const string OnboardingsCreate = "lifecycle.onboardings.create";

    /// <summary>Permission to update onboarding checklists.</summary>
    public const string OnboardingsUpdate = "lifecycle.onboardings.update";

    /// <summary>Permission to read offboarding checklists.</summary>
    public const string OffboardingsRead = "lifecycle.offboardings.read";

    /// <summary>Permission to create offboarding checklists.</summary>
    public const string OffboardingsCreate = "lifecycle.offboardings.create";

    /// <summary>Permission to update offboarding checklists.</summary>
    public const string OffboardingsUpdate = "lifecycle.offboardings.update";

    /// <summary>Permission to read templates.</summary>
    public const string TemplatesRead = "lifecycle.templates.read";

    /// <summary>Permission to create templates.</summary>
    public const string TemplatesCreate = "lifecycle.templates.create";

    /// <summary>Permission to update templates.</summary>
    public const string TemplatesUpdate = "lifecycle.templates.update";

    /// <summary>Permission to delete templates.</summary>
    public const string TemplatesDelete = "lifecycle.templates.delete";

    /// <summary>Permission to read exit interviews.</summary>
    public const string ExitInterviewsRead = "lifecycle.exitinterviews.read";

    /// <summary>Permission to create exit interviews.</summary>
    public const string ExitInterviewsCreate = "lifecycle.exitinterviews.create";

    /// <summary>
    /// Collection of all permissions for easy registration.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, string> All = new Dictionary<string, string>
    {
        // Onboarding
        { OnboardingsRead, "Read onboarding checklists" },
        { OnboardingsCreate, "Create onboarding checklists" },
        { OnboardingsUpdate, "Update onboarding checklists" },

        // Offboarding
        { OffboardingsRead, "Read offboarding checklists" },
        { OffboardingsCreate, "Create offboarding checklists" },
        { OffboardingsUpdate, "Update offboarding checklists" },

        // Templates
        { TemplatesRead, "Read templates" },
        { TemplatesCreate, "Create templates" },
        { TemplatesUpdate, "Update templates" },
        { TemplatesDelete, "Delete templates" },

        // Exit Interviews
        { ExitInterviewsRead, "Read exit interviews" },
        { ExitInterviewsCreate, "Create exit interviews" }
    };
}
