namespace Maliev.LifecycleService.Application.Queries.Offboarding;

/// <summary>
/// Query to retrieve offboarding status for a specific employee.
/// </summary>
/// <param name="EmployeeId">The unique identifier of the employee.</param>
public record GetOffboardingStatusQuery(Guid EmployeeId);
