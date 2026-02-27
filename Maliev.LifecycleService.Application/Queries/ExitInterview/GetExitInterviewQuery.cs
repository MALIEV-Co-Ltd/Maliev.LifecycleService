namespace Maliev.LifecycleService.Application.Queries.ExitInterview;

/// <summary>
/// Query to retrieve exit interview information for a specific employee.
/// </summary>
/// <param name="EmployeeId">The unique identifier of the employee.</param>
public record GetExitInterviewQuery(Guid EmployeeId);
