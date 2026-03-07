using Asp.Versioning;
using Maliev.Aspire.ServiceDefaults.Authorization;
using Maliev.LifecycleService.Application.Commands.Onboarding;
using Maliev.LifecycleService.Application.Commands.Onboarding.Handlers;
using Maliev.LifecycleService.Application.Queries.Onboarding;
using Maliev.LifecycleService.Application.Queries.Onboarding.Handlers;
using Maliev.LifecycleService.Domain.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Maliev.LifecycleService.Api.Controllers;

/// <summary>
/// Controller for managing onboarding workflows.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("lifecycle/v{version:apiVersion}")]
[Authorize]
public class OnboardingController : ControllerBase
{
    private readonly StartOnboardingCommandHandler _startHandler;
    private readonly GetOnboardingStatusQueryHandler _getStatusHandler;
    private readonly GetPendingOnboardingsQueryHandler _getPendingHandler;
    private readonly CompleteOnboardingItemCommandHandler _completeItemHandler;
    private readonly ReassignOnboardingItemCommandHandler _reassignItemHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="OnboardingController"/> class.
    /// </summary>
    /// <param name="startHandler">The start onboarding handler.</param>
    /// <param name="getStatusHandler">The get onboarding status handler.</param>
    /// <param name="getPendingHandler">The get pending onboardings handler.</param>
    /// <param name="completeItemHandler">The complete onboarding item handler.</param>
    /// <param name="reassignItemHandler">The reassign onboarding item handler.</param>
    public OnboardingController(
        StartOnboardingCommandHandler startHandler,
        GetOnboardingStatusQueryHandler getStatusHandler,
        GetPendingOnboardingsQueryHandler getPendingHandler,
        CompleteOnboardingItemCommandHandler completeItemHandler,
        ReassignOnboardingItemCommandHandler reassignItemHandler)
    {
        _startHandler = startHandler;
        _getStatusHandler = getStatusHandler;
        _getPendingHandler = getPendingHandler;
        _completeItemHandler = completeItemHandler;
        _reassignItemHandler = reassignItemHandler;
    }

    /// <summary>
    /// Manually starts an onboarding workflow for a specific employee.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the employee.</param>
    /// <param name="request">The onboarding start request.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A created result with the onboarding workflow identifier.</returns>
    [HttpPost("employees/{employeeId}/onboarding/start")]
    [RequirePermission(LifecyclePermissions.OnboardingsCreate)]
    public async Task<IActionResult> Start(Guid employeeId, [FromBody] StartOnboardingRequest request, CancellationToken ct)
    {
        var userId = GetUserId();
        var id = await _startHandler.HandleAsync(new StartOnboardingCommand(
            employeeId,
            request.StartDate,
            request.TemplateId,
            userId), ct);

        return CreatedAtAction(nameof(GetStatus), new { employeeId }, new { id });
    }

    /// <summary>
    /// Retrieves onboarding progress status for a specific employee.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the employee.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The onboarding status if found.</returns>
    [HttpGet("employees/{employeeId}/onboarding/status")]
    [RequirePermission(LifecyclePermissions.OnboardingsCreate)]
    public async Task<IActionResult> GetStatus(Guid employeeId, CancellationToken ct)
    {
        var result = await _getStatusHandler.HandleAsync(new GetOnboardingStatusQuery(employeeId), ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a paginated list of pending onboarding checklists.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <param name="offset">The number of items to skip.</param>
    /// <param name="limit">The maximum number of items to return.</param>
    /// <returns>A collection of pending onboardings.</returns>
    [HttpGet("onboarding/pending")]
    [RequirePermission(LifecyclePermissions.OnboardingsCreate)]
    public async Task<IActionResult> GetPending(CancellationToken ct, [FromQuery] int offset = 0, [FromQuery] int limit = 50)
    {
        var result = await _getPendingHandler.HandleAsync(new GetPendingOnboardingsQuery(offset, limit), ct);
        return Ok(result);
    }

    /// <summary>
    /// Marks a specific onboarding item as completed.
    /// </summary>
    /// <param name="itemId">The unique identifier of the onboarding item.</param>
    /// <param name="request">The item completion request.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("onboarding-items/{itemId}/complete")]
    [RequirePermission(LifecyclePermissions.OnboardingsUpdate)]
    public async Task<IActionResult> CompleteItem(Guid itemId, [FromBody] CompleteItemRequest request, CancellationToken ct)
    {
        var userId = GetUserId();
        await _completeItemHandler.HandleAsync(new CompleteOnboardingItemCommand(itemId, userId, request.Notes), ct);
        return NoContent();
    }

    /// <summary>
    /// Reassigns a specific onboarding item to another user or role.
    /// </summary>
    /// <param name="itemId">The unique identifier of the onboarding item.</param>
    /// <param name="request">The item reassignment request.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("onboarding-items/{itemId}/reassign")]
    [RequirePermission(LifecyclePermissions.OnboardingsUpdate)]
    public async Task<IActionResult> ReassignItem(Guid itemId, [FromBody] ReassignItemRequest request, CancellationToken ct)
    {
        var userId = GetUserId();
        await _reassignItemHandler.HandleAsync(new ReassignOnboardingItemCommand(itemId, request.AssignedTo, userId), ct);
        return NoContent();
    }

    private Guid GetUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
    }
}
