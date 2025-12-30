using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;

namespace Maliev.LifecycleService.Application.Commands.Templates.Handlers;

/// <summary>
/// Handles the updating of an onboarding template.
/// </summary>
public class UpdateTemplateCommandHandler
{
    private readonly ITemplateRepository _repository;
    private readonly ITemplateCacheService _cache;
    private readonly IAuditLogService _auditLogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTemplateCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The template repository.</param>
    /// <param name="cache">The template cache service.</param>
    /// <param name="auditLogService">The audit logging service.</param>
    public UpdateTemplateCommandHandler(ITemplateRepository repository, ITemplateCacheService cache, IAuditLogService auditLogService)
    {
        _repository = repository;
        _cache = cache;
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// Handles the command to update an onboarding template.
    /// </summary>
    /// <param name="command">The update template command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown when the template is not found.</exception>
    public async Task HandleAsync(UpdateTemplateCommand command, CancellationToken cancellationToken = default)
    {
        var template = await _repository.GetByIdWithItemsAsync(command.Id, cancellationToken);
        if (template == null) throw new Exception("Template not found");

        var beforeState = template;

        template.Name = command.Name;
        template.Description = command.Description;
        template.DepartmentId = command.DepartmentId;
        template.IsActive = command.IsActive;
        template.ModifiedDate = DateTime.UtcNow;

        // Simplify item update: clear and rebuild for MVP
        // In real app, we would match IDs to update/delete/add
        template.Items.Clear();
        foreach (var item in command.Items)
        {
            template.Items.Add(new OnboardingTemplateItem
            {
                Id = item.Id ?? Guid.NewGuid(),
                TemplateId = template.Id,
                Title = item.Title,
                Description = item.Description,
                Category = item.Category,
                DefaultAssigneeRole = item.DefaultAssigneeRole,
                DaysDue = item.DaysDue,
                SortOrder = item.SortOrder
            });
        }

        await _repository.UpdateAsync(template, cancellationToken);

        // Invalidate cache
        await _cache.RemoveAsync(template.DepartmentId, cancellationToken);

        await _auditLogService.LogAsync("OnboardingTemplate", template.Id, "Updated", command.UserId, beforeState, template, cancellationToken);
    }
}