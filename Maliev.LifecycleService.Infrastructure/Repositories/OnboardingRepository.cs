using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;
using Maliev.LifecycleService.Domain.Enums;
using Maliev.LifecycleService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maliev.LifecycleService.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for onboarding checklists.
/// </summary>
public class OnboardingRepository : IOnboardingRepository
{
    private readonly LifecycleDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="OnboardingRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public OnboardingRepository(LifecycleDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task AddAsync(OnboardingChecklist checklist, CancellationToken cancellationToken = default)
    {
        await _context.OnboardingChecklists.AddAsync(checklist, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(OnboardingChecklist checklist, CancellationToken cancellationToken = default)
    {
        _context.OnboardingChecklists.Update(checklist);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<OnboardingChecklist?> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.OnboardingChecklists
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<OnboardingChecklist?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.OnboardingChecklists
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<OnboardingChecklist>> GetPendingAsync(int offset, int limit, CancellationToken cancellationToken = default)
    {
        return await _context.OnboardingChecklists
            .Where(x => x.Status != OnboardingStatus.Completed && x.Status != OnboardingStatus.Cancelled)
            .OrderByDescending(x => x.StartDate)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<OnboardingItem?> GetItemByIdAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        return await _context.OnboardingItems
            .Include(x => x.Checklist)
            .ThenInclude(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == itemId, cancellationToken);
    }
}
