using System.ComponentModel.DataAnnotations;

namespace Maliev.LifecycleService.Application.Validators;

/// <summary>
/// Validates input data for starting an onboarding workflow.
/// </summary>
public class StartOnboardingValidator
{
    /// <summary>
    /// Validates the start onboarding request parameters.
    /// </summary>
    /// <param name="employeeId">The identifier of the employee.</param>
    /// <param name="startDate">The scheduled start date.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(Guid employeeId, DateTime startDate)
    {
        if (employeeId == Guid.Empty)
            yield return new ValidationResult("EmployeeId is required", new[] { nameof(employeeId) });

        if (startDate == default)
            yield return new ValidationResult("StartDate is required", new[] { nameof(startDate) });
    }
}
