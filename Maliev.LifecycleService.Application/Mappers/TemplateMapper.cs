using Maliev.LifecycleService.Application.DTOs;
using Maliev.LifecycleService.Domain.Entities;

namespace Maliev.LifecycleService.Application.Mappers;

/// <summary>
/// Provides extension methods for mapping template-related entities to DTOs.
/// </summary>
public static class TemplateMapper
{
    /// <summary>
    /// Maps an <see cref="OnboardingTemplate"/> entity to a <see cref="TemplateDto"/>.
    /// </summary>
    /// <param name="template">The onboarding template entity to map.</param>
    /// <returns>The mapped template DTO.</returns>
    public static TemplateDto ToDto(this OnboardingTemplate template)
    {
        return new TemplateDto(
            template.Id,
            template.Name,
            template.Description,
            template.DepartmentId,
            template.IsActive,
            template.Items.Select(ToDto).ToList()
        );
    }

    /// <summary>
    /// Maps an <see cref="OnboardingTemplateItem"/> entity to a <see cref="TemplateItemDto"/>.
    /// </summary>
    /// <param name="item">The onboarding template item entity to map.</param>
    /// <returns>The mapped template item DTO.</returns>
    public static TemplateItemDto ToDto(this OnboardingTemplateItem item)
    {
        return new TemplateItemDto(
            item.Id,
            item.Title,
            item.Description,
            item.Category,
            item.DefaultAssigneeRole,
            item.DaysDue,
            item.SortOrder
        );
    }
}
