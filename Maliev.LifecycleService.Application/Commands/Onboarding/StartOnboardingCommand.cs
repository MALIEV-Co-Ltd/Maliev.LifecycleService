namespace Maliev.LifecycleService.Application.Commands.Onboarding;

/// <summary>
/// Command to manually start an onboarding workflow for an employee.
/// </summary>
/// <param name="EmployeeId">The unique identifier of the employee being onboarded.</param>
/// <param name="StartDate">The employee's scheduled start date.</param>
/// <param name="TemplateId">The optional identifier of the onboarding template to use.</param>
/// <param name="UserId">The identifier of the user starting the onboarding.</param>
public record StartOnboardingCommand(
    Guid EmployeeId,
    DateTime StartDate,
    Guid? TemplateId,
    Guid UserId);
