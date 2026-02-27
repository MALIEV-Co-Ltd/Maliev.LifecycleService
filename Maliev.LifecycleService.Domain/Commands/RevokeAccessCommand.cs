using MediatR;

namespace Maliev.LifecycleService.Domain.Commands;

/// <summary>
/// Saga command to revoke all system access for an employee.
/// </summary>
public record RevokeAccessCommand(Guid EmployeeId, Guid CorrelationId) : IRequest<bool>;
