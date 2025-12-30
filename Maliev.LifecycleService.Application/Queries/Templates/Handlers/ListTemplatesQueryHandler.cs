using Maliev.LifecycleService.Application.DTOs;
using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Application.Mappers;

namespace Maliev.LifecycleService.Application.Queries.Templates.Handlers;

/// <summary>
/// Handles the retrieval of a list of all onboarding templates.
/// </summary>
public class ListTemplatesQueryHandler
{
    private readonly ITemplateRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListTemplatesQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The template repository.</param>
    public ListTemplatesQueryHandler(ITemplateRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles the query to list all templates.
    /// </summary>
    /// <param name="query">The list templates query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of template DTOs.</returns>
    public async Task<IEnumerable<TemplateDto>> HandleAsync(ListTemplatesQuery query, CancellationToken cancellationToken = default)
    {
        var templates = await _repository.GetAllAsync(cancellationToken);
        return templates.Select(x => x.ToDto());
    }
}