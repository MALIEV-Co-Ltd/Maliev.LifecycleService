namespace Maliev.LifecycleService.Application.DTOs;

/// <summary>
/// Data transfer object for exit interview information.
/// </summary>
/// <param name="Id">The unique identifier of the exit interview.</param>
/// <param name="OffboardingChecklistId">The associated offboarding checklist identifier.</param>
/// <param name="ConductedBy">The identifier of the user who conducted the interview.</param>
/// <param name="InterviewDate">The date when the interview was conducted.</param>
/// <param name="ReasonForLeaving">The reason provided for leaving.</param>
/// <param name="FeedbackOnManager">Feedback regarding the manager.</param>
/// <param name="FeedbackOnTeam">Feedback regarding the team.</param>
/// <param name="FeedbackOnCompany">Feedback regarding the company.</param>
/// <param name="ImprovementSuggestions">Suggestions for improvement.</param>
/// <param name="WouldRecommendCompany">A value indicating whether the employee would recommend the company.</param>
public record ExitInterviewDto(
    Guid Id,
    Guid OffboardingChecklistId,
    Guid ConductedBy,
    DateTime InterviewDate,
    string? ReasonForLeaving,
    string? FeedbackOnManager,
    string? FeedbackOnTeam,
    string? FeedbackOnCompany,
    string? ImprovementSuggestions,
    bool WouldRecommendCompany);