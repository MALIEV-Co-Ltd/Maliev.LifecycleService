using Maliev.LifecycleService.Domain.Enums;

namespace Maliev.LifecycleService.Application.Commands.Templates;

/// <summary>
/// Command to update an existing onboarding template.
/// </summary>
/// <param name="Id">The unique identifier of the template to update.</param>
/// <param name="Name">The updated name of the template.</param>
/// <param name="Description">The updated description of the template.</param>
/// <param name="DepartmentId">The updated department identifier.</param>
/// <param name="IsActive">A value indicating whether the template is active.</param>
/// <param name="Items">The updated collection of items for the template.</param>
/// <param name="UserId">The identifier of the user performing the update.</param>
public record UpdateTemplateCommand(
    Guid Id,
    string Name,
    string? Description,
    Guid? DepartmentId,
    bool IsActive,
    List<UpdateTemplateItemDto> Items,
    Guid UserId);

/// <summary>
/// Represents an item to be updated or created within an existing template.
/// </summary>
/// <param name="Id">The unique identifier of the item, or null if it's a new item.</param>
/// <param name="Title">The title of the item.</param>
/// <param name="Description">A description of what the item entails.</param>
/// <param name="Category">The category of the item.</param>
/// <param name="DefaultAssigneeRole">The default role assigned to this item.</param>
/// <param name="DaysDue">Number of days after start date that this item is due.</param>
/// <param name="SortOrder">The display order of the item.</param>
public record UpdateTemplateItemDto(
    Guid? Id,
    string Title,
    string? Description,
    ItemCategory Category,
    string? DefaultAssigneeRole,
    int DaysDue,
    int SortOrder);
