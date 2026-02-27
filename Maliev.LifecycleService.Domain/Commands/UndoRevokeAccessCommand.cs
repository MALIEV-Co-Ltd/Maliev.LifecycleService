namespace Maliev.LifecycleService.Domain.Commands;

/// <summary>
/// Command to undo access revocation for an employee.
/// </summary>
public record UndoRevokeAccessCommand
{
    /// <summary>
    /// Gets the unique identifier of the employee.
    /// </summary>
    public Guid EmployeeId { get; init; }
}
