using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;
using Maliev.LifecycleService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maliev.LifecycleService.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for onboarding templates.
/// </summary>
public class TemplateRepository : ITemplateRepository
{
    private readonly LifecycleDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public TemplateRepository(LifecycleDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<OnboardingTemplate?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.OnboardingTemplates
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<OnboardingTemplate?> GetByDepartmentIdAsync(Guid? departmentId, CancellationToken cancellationToken = default)
    {
        // Try to find department specific active template first
        var template = await _context.OnboardingTemplates
            .Include(x => x.Items)
            .Where(x => x.IsActive && x.DepartmentId == departmentId)
            .FirstOrDefaultAsync(cancellationToken);

        // Fallback to default active template if not found and department was specified
        if (template == null && departmentId != null)
        {
            template = await _context.OnboardingTemplates
                .Include(x => x.Items)
                .Where(x => x.IsActive && x.DepartmentId == null)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return template;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<OnboardingTemplate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OnboardingTemplates
            .Include(x => x.Items)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task AddAsync(OnboardingTemplate template, CancellationToken cancellationToken = default)
    {
        await _context.OnboardingTemplates.AddAsync(template, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(OnboardingTemplate template, CancellationToken cancellationToken = default)
    {
        _context.OnboardingTemplates.Update(template);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
