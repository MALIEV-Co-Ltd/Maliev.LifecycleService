using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;
using Maliev.LifecycleService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maliev.LifecycleService.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for exit interviews.
/// </summary>
public class ExitInterviewRepository : IExitInterviewRepository
{
    private readonly LifecycleDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExitInterviewRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ExitInterviewRepository(LifecycleDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task AddAsync(ExitInterview exitInterview, CancellationToken cancellationToken = default)
    {
        await _context.ExitInterviews.AddAsync(exitInterview, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<ExitInterview?> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.ExitInterviews
            .Include(x => x.OffboardingChecklist)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.OffboardingChecklist.EmployeeId == employeeId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<ExitInterview?> GetByChecklistIdAsync(Guid checklistId, CancellationToken cancellationToken = default)
    {
        return await _context.ExitInterviews
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.OffboardingChecklistId == checklistId, cancellationToken);
    }
}
