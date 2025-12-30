namespace Maliev.LifecycleService.Domain.Events;

/// <summary>
/// Event published when a new employee record is created.
/// Trigger for automated onboarding.
/// </summary>
/// <param name="EmployeeId">The unique identifier of the created employee.</param>
/// <param name="EmployeeNumber">The business-specific employee number.</param>
/// <param name="StartDate">The start date of the employee.</param>
/// <param name="DepartmentId">The unique identifier of the employee's department.</param>
/// <param name="PositionId">The unique identifier of the employee's position.</param>
/// <param name="ManagerId">The unique identifier of the employee's manager.</param>
public record EmployeeCreatedEvent(
    Guid EmployeeId,
    string EmployeeNumber,
    DateTime StartDate,
    Guid DepartmentId,
    Guid? PositionId,
    Guid? ManagerId);