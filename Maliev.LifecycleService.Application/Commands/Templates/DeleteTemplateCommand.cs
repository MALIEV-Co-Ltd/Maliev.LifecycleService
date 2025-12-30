namespace Maliev.LifecycleService.Application.Commands.Templates;

/// <summary>
/// Command to delete (soft-delete) an onboarding template.
/// </summary>
/// <param name="Id">The unique identifier of the template to delete.</param>
/// <param name="UserId">The identifier of the user performing the deletion.</param>
public record DeleteTemplateCommand(Guid Id, Guid UserId);