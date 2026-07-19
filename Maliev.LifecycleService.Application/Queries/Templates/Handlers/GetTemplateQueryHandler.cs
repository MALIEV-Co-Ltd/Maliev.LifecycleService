using Maliev.LifecycleService.Application.DTOs;
using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Application.Mappers;

namespace Maliev.LifecycleService.Application.Queries.Templates.Handlers;

/// <summary>
/// Handles the retrieval of a specific onboarding template.
/// </summary>
public class GetTemplateQueryHandler
{
    private readonly ITemplateRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTemplateQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The template repository.</param>
    public GetTemplateQueryHandler(ITemplateRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles the query to get a template.
    /// </summary>
    /// <param name="query">The get template query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The template DTO if found; otherwise, null.</returns>
    public async Task<TemplateDto?> HandleAsync(GetTemplateQuery query, CancellationToken cancellationToken = default)
    {
        var template = await _repository.GetByIdWithItemsAsync(query.Id, cancellationToken);
        return template?.ToDto();
    }
}
