namespace Maliev.LifecycleService.Domain.Entities;

/// <summary>
/// Immutable audit trail for all state-changing lifecycle operations.
/// Retains for 7 years for compliance (FR-025c).
/// </summary>
public class AuditLog
{
    /// <summary>
    /// Unique log entry identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User who performed the action
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// When the action occurred
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Entity class name (e.g., "OnboardingChecklist")
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// ID of the affected entity
    /// </summary>
    public Guid EntityId { get; set; }

    /// <summary>
    /// Action performed (Created, Updated, Deleted, Completed, Reassigned)
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// JSON snapshot of entity state before change (NULL for Created)
    /// </summary>
    public string? BeforeState { get; set; }

    /// <summary>
    /// JSON snapshot of entity state after change (NULL for Deleted)
    /// </summary>
    public string? AfterState { get; set; }
}
