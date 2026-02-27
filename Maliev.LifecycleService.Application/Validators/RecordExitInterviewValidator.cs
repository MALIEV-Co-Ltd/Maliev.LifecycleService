using System.ComponentModel.DataAnnotations;

namespace Maliev.LifecycleService.Application.Validators;

/// <summary>
/// Validates input data for recording an exit interview.
/// </summary>
public class RecordExitInterviewValidator
{
    /// <summary>
    /// Validates the record exit interview request parameters.
    /// </summary>
    /// <param name="employeeId">The identifier of the employee.</param>
    /// <param name="interviewDate">The date of the interview.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(Guid employeeId, DateTime interviewDate)
    {
        if (employeeId == Guid.Empty)
            yield return new ValidationResult("EmployeeId is required", new[] { nameof(employeeId) });

        if (interviewDate == default)
            yield return new ValidationResult("InterviewDate is required", new[] { nameof(interviewDate) });
    }
}
