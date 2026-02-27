using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;
using Maliev.LifecycleService.Domain.Enums;
using Maliev.LifecycleService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maliev.LifecycleService.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for offboarding checklists.
/// </summary>
public class OffboardingRepository : IOffboardingRepository
{
    private readonly LifecycleDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="OffboardingRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public OffboardingRepository(LifecycleDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task AddAsync(OffboardingChecklist checklist, CancellationToken cancellationToken = default)
    {
        await _context.OffboardingChecklists.AddAsync(checklist, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(OffboardingChecklist checklist, CancellationToken cancellationToken = default)
    {
        _context.OffboardingChecklists.Update(checklist);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<OffboardingChecklist?> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.OffboardingChecklists
            .Include(x => x.Tasks)
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<OffboardingChecklist?> GetByIdWithTasksAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.OffboardingChecklists
            .Include(x => x.Tasks)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<OffboardingChecklist>> GetPendingAsync(int offset, int limit, CancellationToken cancellationToken = default)
    {
        return await _context.OffboardingChecklists
            .Where(x => x.Status != OffboardingStatus.Completed && x.Status != OffboardingStatus.Cancelled)
            .OrderByDescending(x => x.TerminationDate)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<OffboardingTask?> GetTaskByIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _context.OffboardingTasks
            .Include(x => x.Checklist)
            .ThenInclude(x => x.Tasks)
            .FirstOrDefaultAsync(x => x.Id == taskId, cancellationToken);
    }
}
