using Maliev.LifecycleService.Domain.Enums;

namespace Maliev.LifecycleService.Application.DTOs;

/// <summary>
/// Data transfer object representing the status of an offboarding process.
/// </summary>
/// <param name="Id">The unique identifier of the offboarding checklist.</param>
/// <param name="EmployeeId">The identifier of the employee being offboarded.</param>
/// <param name="TerminationDate">The scheduled termination date.</param>
/// <param name="TerminationReason">The reason for termination.</param>
/// <param name="EligibleForRehire">A value indicating whether the employee is eligible for rehire.</param>
/// <param name="Status">The current status of the offboarding process.</param>
/// <param name="PaycheckReleaseBlocked">A value indicating whether the final paycheck release is currently blocked.</param>
/// <param name="CompletedDate">The date when the offboarding process was completed.</param>
/// <param name="Tasks">The collection of tasks associated with this offboarding process.</param>
public record OffboardingStatusDto(
    Guid Id,
    Guid EmployeeId,
    DateTime TerminationDate,
    string TerminationReason,
    bool EligibleForRehire,
    OffboardingStatus Status,
    bool PaycheckReleaseBlocked,
    DateTime? CompletedDate,
    List<OffboardingTaskDto> Tasks);