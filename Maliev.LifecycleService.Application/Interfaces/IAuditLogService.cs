using Maliev.LifecycleService.Domain.Entities;

namespace Maliev.LifecycleService.Application.Interfaces;

/// <summary>
/// Defines the service for recording and retrieving audit logs.
/// </summary>
public interface IAuditLogService
{
    /// <summary>
    /// Records a new audit log entry.
    /// </summary>
    /// <param name="entityType">The type of the entity being audited.</param>
    /// <param name="entityId">The identifier of the entity being audited.</param>
    /// <param name="action">The action performed.</param>
    /// <param name="userId">The identifier of the user who performed the action.</param>
    /// <param name="beforeState">The optional state of the entity before the action.</param>
    /// <param name="afterState">The optional state of the entity after the action.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task LogAsync(string entityType, Guid entityId, string action, Guid userId, object? beforeState = null, object? afterState = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an audit log entry by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the audit log.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The audit log entry if found; otherwise, null.</returns>
    Task<AuditLog?> GetLogAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves audit log entries for a specific entity.
    /// </summary>
    /// <param name="entityType">The type of the entity.</param>
    /// <param name="entityId">The identifier of the entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of audit log entries for the entity.</returns>
    Task<IEnumerable<AuditLog>> GetLogsForEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default);
}
