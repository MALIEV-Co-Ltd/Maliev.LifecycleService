using Maliev.LifecycleService.Domain.Enums;

namespace Maliev.LifecycleService.Application.Commands.Templates;

/// <summary>
/// Command to create a new onboarding template.
/// </summary>
/// <param name="Name">The name of the template.</param>
/// <param name="Description">A description of the template.</param>
/// <param name="DepartmentId">The optional department identifier this template is specific to.</param>
/// <param name="Items">The collection of items to include in the template.</param>
/// <param name="UserId">The identifier of the user creating the template.</param>
public record CreateTemplateCommand(
    string Name,
    string? Description,
    Guid? DepartmentId,
    List<CreateTemplateItemDto> Items,
    Guid UserId);

/// <summary>
/// Represents an item to be created within a template.
/// </summary>
/// <param name="Title">The title of the item.</param>
/// <param name="Description">A description of what the item entails.</param>
/// <param name="Category">The category of the item.</param>
/// <param name="DefaultAssigneeRole">The default role assigned to this item.</param>
/// <param name="DaysDue">Number of days after start date that this item is due.</param>
/// <param name="SortOrder">The display order of the item.</param>
public record CreateTemplateItemDto(
    string Title,
    string? Description,
    ItemCategory Category,
    string? DefaultAssigneeRole,
    int DaysDue,
    int SortOrder);