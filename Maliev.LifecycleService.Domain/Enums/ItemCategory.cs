namespace Maliev.LifecycleService.Domain.Enums;

/// <summary>
/// Categorizes onboarding items for organization and filtering.
/// </summary>
public enum ItemCategory
{
    /// <summary>
    /// Paperwork, forms, agreements
    /// </summary>
    Documentation = 0,

    /// <summary>
    /// Equipment, accounts, access
    /// </summary>
    ITSetup = 1,

    /// <summary>
    /// Orientation, skill training
    /// </summary>
    Training = 2,

    /// <summary>
    /// Legal, regulatory requirements
    /// </summary>
    Compliance = 3,

    /// <summary>
    /// Meet and greets, team onboarding
    /// </summary>
    TeamIntroduction = 4,

    /// <summary>
    /// General admin tasks
    /// </summary>
    Administrative = 5
}
