namespace Maliev.LifecycleService.Application.Queries.Onboarding;

/// <summary>
/// Query to retrieve onboarding status for a specific employee.
/// </summary>
/// <param name="EmployeeId">The unique identifier of the employee.</param>
public record GetOnboardingStatusQuery(Guid EmployeeId);