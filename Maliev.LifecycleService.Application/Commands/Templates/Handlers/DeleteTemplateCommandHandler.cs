using Maliev.LifecycleService.Application.Interfaces;

namespace Maliev.LifecycleService.Application.Commands.Templates.Handlers;

/// <summary>
/// Handles the deletion of an onboarding template.
/// </summary>
public class DeleteTemplateCommandHandler
{
    private readonly ITemplateRepository _repository;
    private readonly ITemplateCacheService _cache;
    private readonly IAuditLogService _auditLogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteTemplateCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The template repository.</param>
    /// <param name="cache">The template cache service.</param>
    /// <param name="auditLogService">The audit logging service.</param>
    public DeleteTemplateCommandHandler(ITemplateRepository repository, ITemplateCacheService cache, IAuditLogService auditLogService)
    {
        _repository = repository;
        _cache = cache;
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// Handles the command to delete an onboarding template.
    /// </summary>
    /// <param name="command">The delete template command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task HandleAsync(DeleteTemplateCommand command, CancellationToken cancellationToken = default)
    {
        var template = await _repository.GetByIdWithItemsAsync(command.Id, cancellationToken);
        if (template == null) return;

        template.IsActive = false;
        template.ModifiedDate = DateTime.UtcNow;

        await _repository.UpdateAsync(template, cancellationToken);

        // Invalidate cache
        await _cache.RemoveAsync(template.DepartmentId, cancellationToken);

        await _auditLogService.LogAsync("OnboardingTemplate", template.Id, "SoftDeleted", command.UserId, null, null, cancellationToken);
    }
}