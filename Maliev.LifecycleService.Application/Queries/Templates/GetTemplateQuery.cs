namespace Maliev.LifecycleService.Application.Queries.Templates;

/// <summary>
/// Query to retrieve a specific onboarding template by its identifier.
/// </summary>
/// <param name="Id">The unique identifier of the template.</param>
public record GetTemplateQuery(Guid Id);
