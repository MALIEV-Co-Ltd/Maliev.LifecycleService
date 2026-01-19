using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Maliev.LifecycleService.Application.Services;

/// <summary>
/// Provides methods for logging and retrieving audit trail information.
/// </summary>
public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _repository;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditLogService"/> class.
    /// </summary>
    /// <param name="repository">The audit log repository.</param>
    public AuditLogService(IAuditLogRepository repository)
    {
        _repository = repository;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = false
        };
    }

    /// <inheritdoc/>
    public async Task LogAsync(string entityType, Guid entityId, string action, Guid userId, object? beforeState = null, object? afterState = null, CancellationToken cancellationToken = default)
    {
        // Simple projection to avoid deep graphs in audit log
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