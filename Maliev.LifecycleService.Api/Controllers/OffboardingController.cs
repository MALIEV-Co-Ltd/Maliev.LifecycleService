using Asp.Versioning;
using Maliev.Aspire.ServiceDefaults.Authorization;
using Maliev.LifecycleService.Application.Commands.Offboarding;
using Maliev.LifecycleService.Application.Commands.Offboarding.Handlers;
using Maliev.LifecycleService.Application.Queries.Offboarding;
using Maliev.LifecycleService.Application.Queries.Offboarding.Handlers;
using Maliev.LifecycleService.Domain.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Maliev.LifecycleService.Api.Controllers;

/// <summary>
/// Controller for managing offboarding workflows.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("lifecycle/v{version:apiVersion}")]
[Authorize]
public class OffboardingController : ControllerBase
{
    private readonly StartOffboardingCommandHandler _startHandler;
    private readonly GetOffboardingStatusQueryHandler _getStatusHandler;
    private readonly CompleteOffboardingTaskCommandHandler _completeTaskHandler;
    private readonly ReassignOffboardingTaskCommandHandler _reassignTaskHandler;
    private readonly GetPendingOffboardingsQueryHandler _getPendingHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="OffboardingController"/> class.
    /// </summary>
    /// <param name="startHandler">The start offboarding handler.</param>
    /// <param name="getStatusHandler">The get offboarding status handler.</param>
    /// <param name="completeTaskHandler">The complete offboarding task handler.</param>
    /// <param name="reassignTaskHandler">The reassign offboarding task handler.</param>
    /// <param name="getPendingHandler">The get pending offboardings handler.</param>
    public OffboardingController(
        StartOffboardingCommandHandler startHandler,
        GetOffboardingStatusQueryHandler getStatusHandler,
        CompleteOffboardingTaskCommandHandler completeTaskHandler,
        ReassignOffboardingTaskCommandHandler reassignTaskHandler,
        GetPendingOffboardingsQueryHandler getPendingHandler)
    {
        _startHandler = startHandler;
        _getStatusHandler = getStatusHandler;
        _completeTaskHandler = completeTaskHandler;
        _reassignTaskHandler = reassignTaskHandler;
        _getPendingHandler = getPendingHandler;
    }

    /// <summary>
    /// Starts an offboarding workflow for a specific employee.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the employee.</param>
    /// <param name="request">The offboarding start request.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A created result with the offboarding workflow identifier.</returns>
    [HttpPost("employees/{employeeId}/offboarding/start")]
    [RequirePermission(LifecyclePermissions.OffboardingsCreate)]
    public async Task<IActionResult> Start(Guid employeeId, [FromBody] StartOffboardingRequest request, CancellationToken ct)
    {
        var userId = GetUserId();
        var id = await _startHandler.HandleAsync(new StartOffboardingCommand(
            employeeId,
            request.TerminationDate,
            request.TerminationReason,
            request.EligibleForRehire,
            userId), ct);

        return CreatedAtAction(nameof(GetStatus), new { employeeId }, new { id });
    }

    /// <summary>
    /// Retrieves offboarding status for a specific employee.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the employee.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The offboarding status if found.</returns>
    [HttpGet("employees/{employeeId}/offboarding/status")]
    [RequirePermission(LifecyclePermissions.OffboardingsRead)]
    public async Task<IActionResult> GetStatus(Guid employeeId, CancellationToken ct)
    {
        var result = await _getStatusHandler.HandleAsync(new GetOffboardingStatusQuery(employeeId), ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a paginated list of pending offboarding workflows.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <param name="offset">The number of items to skip.</param>
    /// <param name="limit">The maximum number of items to return.</param>
    /// <returns>A collection of pending offboardings.</returns>
    [HttpGet("offboarding/pending")]
    [RequirePermission(LifecyclePermissions.OffboardingsRead)]
    public async Task<IActionResult> GetPending(CancellationToken ct, [FromQuery] int offset = 0, [FromQuery] int limit = 50)
    {
        var result = await _getPendingHandler.HandleAsync(new GetPendingOffboardingsQuery(offset, limit), ct);
        return Ok(result);
    }

    /// <summary>
    /// Marks a specific offboarding task as completed.
    /// </summary>
    /// <param name="taskId">The unique identifier of the offboarding task.</param>
    /// <param name="request">The task completion request.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("offboarding-tasks/{taskId}/complete")]
    [RequirePermission(LifecyclePermissions.OffboardingsUpdate)]
    public async Task<IActionResult> CompleteTask(Guid taskId, [FromBody] CompleteTaskRequest request, CancellationToken ct)
    {
        var userId = GetUserId();
        await _completeTaskHandler.HandleAsync(new CompleteOffboardingTaskCommand(taskId, userId, request.Notes), ct);
        return NoContent();
    }

    /// <summary>
    /// Reassigns a specific offboarding task to another user or role.
    /// </summary>
    /// <param name="taskId">The unique identifier of the offboarding task.</param>
    /// <param name="request">The task reassignment request.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("offboarding-tasks/{taskId}/reassign")]
    [RequirePermission(LifecyclePermissions.OffboardingsUpdate)]
    public async Task<IActionResult> ReassignTask(Guid taskId, [FromBody] ReassignTaskRequest request, CancellationToken ct)
    {
        var userId = GetUserId();
        await _reassignTaskHandler.HandleAsync(new ReassignOffboardingTaskCommand(taskId, request.AssignedTo, userId), ct);
        return NoContent();
    }

    private Guid GetUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
    }
}

/// <summary>
/// Request to start an offboarding workflow.
/// </summary>
/// <param name="TerminationDate">The scheduled termination date.</param>
/// <param name="TerminationReason">The reason for termination.</param>
/// <param name="EligibleForRehire">A value indicating whether the employee is eligible for rehire.</param>
public record StartOffboardingRequest(DateTime TerminationDate, string TerminationReason, bool EligibleForRehire);

/// <summary>
/// Request to complete an offboarding task.
/// </summary>
/// <param name="Notes">Optional notes regarding task completion.</param>
public record CompleteTaskRequest(string? Notes);

/// <summary>
/// Request to reassign an offboarding task.
/// </summary>
/// <param name="AssignedTo">The identifier of the new assignee.</param>
public record ReassignTaskRequest(Guid AssignedTo);
