namespace Maliev.LifecycleService.Application.Commands.ExitInterview;

/// <summary>
/// Command to record an exit interview for an employee.
/// </summary>
/// <param name="EmployeeId">The unique identifier of the employee.</param>
/// <param name="ConductedBy">The identifier of the person who conducted the interview.</param>
/// <param name="InterviewDate">The date when the interview was conducted.</param>
/// <param name="ReasonForLeaving">The reason provided for leaving the company.</param>
/// <param name="FeedbackOnManager">Feedback regarding the employee's manager.</param>
/// <param name="FeedbackOnTeam">Feedback regarding the employee's team.</param>
/// <param name="FeedbackOnCompany">Feedback regarding the company as a whole.</param>
/// <param name="ImprovementSuggestions">Suggestions for company improvement.</param>
/// <param name="WouldRecommendCompany">A value indicating whether the employee would recommend the company.</param>
/// <param name="UserId">The identifier of the user recording the interview.</param>
public record RecordExitInterviewCommand(
    Guid EmployeeId,
    Guid ConductedBy,
    DateTime InterviewDate,
    string? ReasonForLeaving,
    string? FeedbackOnManager,
    string? FeedbackOnTeam,
    string? FeedbackOnCompany,
    string? ImprovementSuggestions,
    bool WouldRecommendCompany,
    Guid UserId);