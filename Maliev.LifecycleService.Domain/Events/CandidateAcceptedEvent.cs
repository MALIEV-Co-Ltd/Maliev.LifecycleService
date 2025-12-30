namespace Maliev.LifecycleService.Domain.Events;

/// <summary>
/// Event published when a candidate accepts an offer.
/// Can be used for pre-onboarding tasks.
/// </summary>
/// <param name="CandidateId">The unique identifier of the candidate.</param>
/// <param name="Name">The full name of the candidate.</param>
/// <param name="Email">The email address of the candidate.</param>
/// <param name="DepartmentId">The unique identifier of the department the candidate will join.</param>
/// <param name="ExpectedStartDate">The expected start date for the candidate.</param>
public record CandidateAcceptedEvent(
    Guid CandidateId,
    string Name,
    string Email,
    Guid DepartmentId,
    DateTime ExpectedStartDate);