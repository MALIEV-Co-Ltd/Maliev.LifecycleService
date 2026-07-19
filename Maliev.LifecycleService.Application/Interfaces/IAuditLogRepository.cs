using Maliev.LifecycleService.Domain.Entities;

namespace Maliev.LifecycleService.Application.Interfaces;

/// <summary>
/// Defines the repository for persisting and retrieving audit logs.
/// </summary>
public interface IAuditLogRepository
{
    /// <summary>
    /// Adds a new audit log entry.
    /// </summary>
    /// <param name="log">The audit log entry to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(AuditLog log, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an audit log entry by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the audit log.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The audit log entry if found; otherwise, null.</returns>
    Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves audit log entries for a specific entity.
    /// </summary>
    /// <param name="entityType">The type of the entity.</param>
    /// <param name="entityId">The identifier of the entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of audit log entries for the entity.</returns>
    Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default);
}
