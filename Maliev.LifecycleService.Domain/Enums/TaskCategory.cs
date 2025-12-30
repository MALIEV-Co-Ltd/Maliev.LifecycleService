namespace Maliev.LifecycleService.Domain.Enums;

/// <summary>
/// Categorizes offboarding tasks for organization and filtering.
/// </summary>
public enum TaskCategory
{
    /// <summary>
    /// Exit paperwork
    /// </summary>
    Documentation = 0,

    /// <summary>
    /// Revoke accounts, credentials
    /// </summary>
    ITAccess = 1,

    /// <summary>
    /// Return laptop, badge, etc.
    /// </summary>
    EquipmentReturn = 2,

    /// <summary>
    /// Handoff responsibilities
    /// </summary>
    KnowledgeTransfer = 3,

    /// <summary>
    /// Final pay, benefits
    /// </summary>
    Financial = 4,

    /// <summary>
    /// General admin tasks
    /// </summary>
    Administrative = 5
}
