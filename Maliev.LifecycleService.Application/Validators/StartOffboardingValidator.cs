using System.ComponentModel.DataAnnotations;

namespace Maliev.LifecycleService.Application.Validators;

/// <summary>
/// Validates input data for starting an offboarding workflow.
/// </summary>
public class StartOffboardingValidator
{
    /// <summary>
    /// Validates the start offboarding request parameters.
    /// </summary>
    /// <param name="employeeId">The identifier of the employee.</param>
    /// <param name="terminationDate">The scheduled termination date.</param>
    /// <param name="terminationReason">The reason for termination.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(Guid employeeId, DateTime terminationDate, string terminationReason)
    {
        if (employeeId == Guid.Empty)
            yield return new ValidationResult("EmployeeId is required", new[] { nameof(employeeId) });

        if (terminationDate == default)
            yield return new ValidationResult("TerminationDate is required", new[] { nameof(terminationDate) });

        if (string.IsNullOrWhiteSpace(terminationReason))
            yield return new ValidationResult("TerminationReason is required", new[] { nameof(terminationReason) });
    }
}