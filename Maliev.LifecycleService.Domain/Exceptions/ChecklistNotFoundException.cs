namespace Maliev.LifecycleService.Domain.Exceptions;

/// <summary>
/// Exception thrown when a requested checklist or item cannot be found.
/// </summary>
public class ChecklistNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChecklistNotFoundException"/> class with a checklist identifier.
    /// </summary>
    /// <param name="checklistId">The unique identifier of the checklist that was not found.</param>
    public ChecklistNotFoundException(Guid checklistId)
        : base($"Checklist {checklistId} not found.")
    {
        ChecklistId = checklistId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChecklistNotFoundException"/> class with an employee identifier and checklist type.
    /// </summary>
    /// <param name="employeeId">The identifier of the employee whose checklist was not found.</param>
    /// <param name="checklistType">The type of the checklist (e.g., "Onboarding", "Offboarding").</param>
    public ChecklistNotFoundException(Guid employeeId, string checklistType)
        : base($"{checklistType} checklist not found for employee {employeeId}.")
    {
        EmployeeId = employeeId;
        ChecklistType = checklistType;
    }

    /// <summary>
    /// Gets the unique identifier of the checklist that was not found.
    /// </summary>
    public Guid? ChecklistId { get; }

    /// <summary>
    /// Gets the identifier of the employee whose checklist was not found.
    /// </summary>
    public Guid? EmployeeId { get; }

    /// <summary>
    /// Gets the type of the checklist that was not found.
    /// </summary>
    public string? ChecklistType { get; }
}
