namespace Maliev.LifecycleService.Domain.Exceptions;

/// <summary>
/// Exception thrown when attempting to start an onboarding/offboarding checklist that already exists.
/// </summary>
public class ChecklistAlreadyStartedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChecklistAlreadyStartedException"/> class.
    /// </summary>
    /// <param name="employeeId">The identifier of the employee who already has a checklist.</param>
    public ChecklistAlreadyStartedException(Guid employeeId)
        : base($"Checklist already exists for employee {employeeId}. Cannot start a duplicate checklist.")
    {
        EmployeeId = employeeId;
    }

    /// <summary>
    /// Gets the identifier of the employee who already has a checklist.
    /// </summary>
    public Guid EmployeeId { get; }
}
