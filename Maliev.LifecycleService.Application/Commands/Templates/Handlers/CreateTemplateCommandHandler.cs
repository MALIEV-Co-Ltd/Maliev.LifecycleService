using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;

namespace Maliev.LifecycleService.Application.Commands.Templates.Handlers;

/// <summary>
/// Handles the creation of an onboarding template.
/// </summary>
public class CreateTemplateCommandHandler
{
    private readonly ITemplateRepository _repository;
    private readonly ITemplateCacheService _cache;
    private readonly IAuditLogService _auditLogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTemplateCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The template repository.</param>
    /// <param name="cache">The template cache service.</param>
    /// <param name="auditLogService">The audit logging service.</param>
    public CreateTemplateCommandHandler(ITemplateRepository repository, ITemplateCacheService cache, IAuditLogService auditLogService)
    {
        _repository = repository;
        _cache = cache;
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// Handles the command to create an onboarding template.
    /// </summary>
    /// <param name="command">The create template command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The unique identifier of the created template.</returns>
    public async Task<Guid> HandleAsync(CreateTemplateCommand command, CancellationToken cancellationToken = default)
    {
        var template = new OnboardingTemplate
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description,
            DepartmentId = command.DepartmentId,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        foreach (var item in command.Items)
        {
            template.Items.Add(new OnboardingTemplateItem
            {
                Id = Guid.NewGuid(),
                TemplateId = template.Id,
                Title = item.Title,
                Description = item.Description,
                Category = item.Category,
                DefaultAssigneeRole = item.DefaultAssigneeRole,
                DaysDue = item.DaysDue,
                SortOrder = item.SortOrder
            });
        }

        await _repository.AddAsync(template, cancellationToken);

        // Invalidate cache
        await _cache.RemoveAsync(template.DepartmentId, cancellationToken);

        await _auditLogService.LogAsync("OnboardingTemplate", template.Id, "Created", command.UserId, null, template, cancellationToken);

        return template.Id;
    }
}
