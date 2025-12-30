namespace Maliev.LifecycleService.Application.Commands.Offboarding;

/// <summary>
/// Command to start an offboarding workflow for an employee.
/// </summary>
/// <param name="EmployeeId">The unique identifier of the employee being offboarded.</param>
/// <param name="TerminationDate">The scheduled termination date.</param>
/// <param name="TerminationReason">The reason for termination.</param>
/// <param name="EligibleForRehire">A value indicating whether the employee is eligible for rehire.</param>
/// <param name="UserId">The identifier of the user starting the offboarding.</param>
public record StartOffboardingCommand(
    Guid EmployeeId,
    DateTime TerminationDate,
    string TerminationReason,
    bool EligibleForRehire,
    Guid UserId);