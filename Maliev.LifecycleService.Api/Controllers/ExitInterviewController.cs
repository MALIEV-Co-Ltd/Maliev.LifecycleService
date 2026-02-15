using Maliev.Aspire.ServiceDefaults.Authorization;
using Maliev.LifecycleService.Application.Commands.ExitInterview;
using Maliev.LifecycleService.Application.Commands.ExitInterview.Handlers;
using Maliev.LifecycleService.Application.Queries.ExitInterview;
using Maliev.LifecycleService.Application.Queries.ExitInterview.Handlers;
using Maliev.LifecycleService.Domain.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Maliev.LifecycleService.Api.Controllers;

/// <summary>
/// Controller for managing exit interviews.
/// </summary>
[ApiController]
[Route("lifecycle/v1")]
[Authorize]
public class ExitInterviewController : ControllerBase
{
    private readonly RecordExitInterviewCommandHandler _recordHandler;
    private readonly GetExitInterviewQueryHandler _getHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExitInterviewController"/> class.
    /// </summary>
    /// <param name="recordHandler">The record exit interview handler.</param>
    /// <param name="getHandler">The get exit interview handler.</param>
    public ExitInterviewController(RecordExitInterviewCommandHandler recordHandler, GetExitInterviewQueryHandler getHandler)
    {
        _recordHandler = recordHandler;
        _getHandler = getHandler;
    }

    /// <summary>
    /// Retrieves exit interview information for a specific employee.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the employee.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The exit interview information if found.</returns>
    [HttpGet("employees/{employeeId}/exit-interview")]
    [RequirePermission(LifecyclePermissions.Admin)] // Restricted access as per US4
    public async Task<IActionResult> Get(Guid employeeId, CancellationToken ct)
    {
        var result = await _getHandler.HandleAsync(new GetExitInterviewQuery(employeeId), ct);
        if (result == null) return NotFound();

        // The policy will handle the admin check. Add an additional check for the conductor.
        var userId = GetUserId();
        // Since we are using policy-based auth, we should check if the user has the permission or is the conductor.
        // For simplicity in this context, we check the claim directly or assume the policy allows admins.
        if (!User.HasClaim("permissions", LifecyclePermissions.Admin) && result.ConductedBy != userId)
        {
            return Forbid();
        }

        return Ok(result);
    }

    /// <summary>
    /// Records a new exit interview for an employee.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the employee.</param>
    /// <param name="request">The exit interview recording request.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A created result with the exit interview identifier.</returns>
    [HttpPost("employees/{employeeId}/exit-interview")]
    [RequirePermission(LifecyclePermissions.Manage)]
    public async Task<IActionResult> Record(Guid employeeId, [FromBody] RecordExitInterviewRequest request, CancellationToken ct)
    {
        var userId = GetUserId();
        var id = await _recordHandler.HandleAsync(new RecordExitInterviewCommand(
            employeeId,
            request.ConductedBy,
            request.InterviewDate,
            request.ReasonForLeaving,
            request.FeedbackOnManager,
            request.FeedbackOnTeam,
            request.FeedbackOnCompany,
            request.ImprovementSuggestions,
            request.WouldRecommendCompany,
            userId), ct);

        return CreatedAtAction(nameof(Get), new { employeeId }, new { id });
    }

    private Guid GetUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
    }
}

/// <summary>
/// Request to record an exit interview.
/// </summary>
/// <param name="ConductedBy">The identifier of the user who conducted the interview.</param>
/// <param name="InterviewDate">The date when the interview was conducted.</param>
/// <param name="ReasonForLeaving">The reason provided for leaving.</param>
/// <param name="FeedbackOnManager">Feedback regarding the manager.</param>
/// <param name="FeedbackOnTeam">Feedback regarding the team.</param>
/// <param name="FeedbackOnCompany">Feedback regarding the company.</param>
/// <param name="ImprovementSuggestions">Suggestions for improvement.</param>
/// <param name="WouldRecommendCompany">A value indicating whether the employee would recommend the company.</param>
public record RecordExitInterviewRequest(
    Guid ConductedBy,
    DateTime InterviewDate,
    string? ReasonForLeaving,
    string? FeedbackOnManager,
    string? FeedbackOnTeam,
    string? FeedbackOnCompany,
    string? ImprovementSuggestions,
    bool WouldRecommendCompany);