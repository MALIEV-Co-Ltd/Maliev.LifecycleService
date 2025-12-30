using Maliev.LifecycleService.Domain.Enums;

namespace Maliev.LifecycleService.Application.DTOs;

/// <summary>
/// Data transfer object for an onboarding template.
/// </summary>
/// <param name="Id">The unique identifier of the onboarding template.</param>
/// <param name="Name">The name of the template.</param>
/// <param name="Description">A description of the template.</param>
/// <param name="DepartmentId">The optional department identifier this template is specific to.</param>
/// <param name="IsActive">A value indicating whether the template is active.</param>
/// <param name="Items">The collection of items included in this template.</param>
public record TemplateDto(
    Guid Id,
    string Name,
    string? Description,
    Guid? DepartmentId,
    bool IsActive,
    List<TemplateItemDto> Items);

/// <summary>
/// Data transfer object for an item within an onboarding template.
/// </summary>
/// <param name="Id">The unique identifier of the template item.</param>
/// <param name="Title">The title of the template item.</param>
/// <param name="Description">A description of what the template item entails.</param>
/// <param name="Category">The category of the onboarding item.</param>
/// <param name="DefaultAssigneeRole">The default role assigned to this item.</param>
/// <param name="DaysDue">Number of days after start date that this item is typically due.</param>
/// <param name="SortOrder">The display order of the item.</param>
public record TemplateItemDto(
    Guid Id,
    string Title,
    string? Description,
    ItemCategory Category,
    string? DefaultAssigneeRole,
    int DaysDue,
    int SortOrder);