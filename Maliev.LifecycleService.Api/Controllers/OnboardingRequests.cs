namespace Maliev.LifecycleService.Api.Controllers;

/// <summary>
/// Request to manually start an onboarding workflow.
/// </summary>
/// <param name="StartDate">The scheduled start date for the employee.</param>
/// <param name="TemplateId">The optional identifier of the template to use.</param>
public record StartOnboardingRequest(DateTime StartDate, Guid? TemplateId);

/// <summary>
/// Request to complete an onboarding item.
/// </summary>
/// <param name="Notes">Optional notes regarding item completion.</param>
public record CompleteItemRequest(string? Notes);

/// <summary>
/// Request to reassign an onboarding item.
/// </summary>
/// <param name="AssignedTo">The identifier of the new assignee.</param>
public record ReassignItemRequest(Guid AssignedTo);