using Asp.Versioning;
using Maliev.Aspire.ServiceDefaults.Authorization;
using Maliev.LifecycleService.Application.Commands.Templates;
using Maliev.LifecycleService.Application.Commands.Templates.Handlers;
using Maliev.LifecycleService.Application.Queries.Templates;
using Maliev.LifecycleService.Application.Queries.Templates.Handlers;
using Maliev.LifecycleService.Domain.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Maliev.LifecycleService.Api.Controllers;

/// <summary>
/// Controller for managing onboarding templates.
/// </summary>
[ApiController]
[ApiVersion("1")]
[Route("lifecycle/v{version:apiVersion}/templates")]
[Authorize]
public class TemplatesController : ControllerBase
{
    private readonly CreateTemplateCommandHandler _createHandler;
    private readonly UpdateTemplateCommandHandler _updateHandler;
    private readonly DeleteTemplateCommandHandler _deleteHandler;
    private readonly GetTemplateQueryHandler _getHandler;
    private readonly ListTemplatesQueryHandler _listHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplatesController"/> class.
    /// </summary>
    /// <param name="createHandler">The create template handler.</param>
    /// <param name="updateHandler">The update template handler.</param>
    /// <param name="deleteHandler">The delete template handler.</param>
    /// <param name="getHandler">The get template handler.</param>
    /// <param name="listHandler">The list templates handler.</param>
    public TemplatesController(
        CreateTemplateCommandHandler createHandler,
        UpdateTemplateCommandHandler updateHandler,
        DeleteTemplateCommandHandler deleteHandler,
        GetTemplateQueryHandler getHandler,
        ListTemplatesQueryHandler listHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _getHandler = getHandler;
        _listHandler = listHandler;
    }

    /// <summary>
    /// Retrieves a list of all onboarding templates.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A collection of onboarding templates.</returns>
    [HttpGet]
    [RequirePermission(LifecyclePermissions.TemplatesRead)]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var result = await _listHandler.HandleAsync(new ListTemplatesQuery(), ct);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific onboarding template by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the template.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The onboarding template if found.</returns>
    [HttpGet("{id}")]
    [RequirePermission(LifecyclePermissions.TemplatesRead)]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var result = await _getHandler.HandleAsync(new GetTemplateQuery(id), ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Creates a new onboarding template.
    /// </summary>
    /// <param name="command">The template creation command.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A created result with the template identifier.</returns>
    [HttpPost]
    [RequirePermission(LifecyclePermissions.TemplatesCreate)]
    public async Task<IActionResult> Create([FromBody] CreateTemplateCommand command, CancellationToken ct)
    {
        var userId = GetUserId();
        var id = await _createHandler.HandleAsync(command with { UserId = userId }, ct);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    /// <summary>
    /// Updates an existing onboarding template.
    /// </summary>
    /// <param name="id">The unique identifier of the template to update.</param>
    /// <param name="command">The template update command.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("{id}")]
    [RequirePermission(LifecyclePermissions.TemplatesUpdate)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTemplateCommand command, CancellationToken ct)
    {
        var userId = GetUserId();
        if (id != command.Id) return BadRequest("ID mismatch");

        await _updateHandler.HandleAsync(command with { UserId = userId }, ct);
        return NoContent();
    }

    /// <summary>
    /// Deletes (soft-deletes) an onboarding template.
    /// </summary>
    /// <param name="id">The unique identifier of the template to delete.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id}")]
    [RequirePermission(LifecyclePermissions.TemplatesDelete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var userId = GetUserId();
        await _deleteHandler.HandleAsync(new DeleteTemplateCommand(id, userId), ct);
        return NoContent();
    }

    private Guid GetUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
    }
}
