using System.Text.Json;
using System.Text.Json.Serialization;
using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;

namespace Maliev.LifecycleService.Application.Services;

/// <summary>
/// Provides methods for logging and retrieving audit trail information.
/// </summary>
public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _repository;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = false
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditLogService"/> class.
    /// </summary>
    /// <param name="repository">The audit log repository.</param>
    public AuditLogService(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc/>
    public async Task LogAsync(string entityType, Guid entityId, string action, Guid userId, object? beforeState = null, object? afterState = null, CancellationToken cancellationToken = default)
    {
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            UserId = userId,
            Timestamp = DateTime.UtcNow,
            BeforeState = beforeState != null ? JsonSerializer.Serialize(beforeState, _jsonOptions) : null,
            AfterState = afterState != null ? JsonSerializer.Serialize(afterState, _jsonOptions) : null
        };

        await _repository.AddAsync(log, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<AuditLog?> GetLogAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AuditLog>> GetLogsForEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByEntityAsync(entityType, entityId, cancellationToken);
    }
}