using Maliev.LifecycleService.Domain.Entities;
using Maliev.LifecycleService.Infrastructure.Data.Configurations;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Maliev.LifecycleService.Infrastructure.Data;

/// <summary>
/// Entity Framework database context for the Lifecycle Service.
/// </summary>
public class LifecycleDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LifecycleDbContext"/> class.
    /// </summary>
    /// <param name="options">The context options.</param>
    public LifecycleDbContext(DbContextOptions<LifecycleDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the audit logs.
    /// </summary>
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    /// <summary>
    /// Gets or sets the onboarding checklists.
    /// </summary>
    public DbSet<OnboardingChecklist> OnboardingChecklists { get; set; } = null!;

    /// <summary>
    /// Gets or sets the individual onboarding items.
    /// </summary>
    public DbSet<OnboardingItem> OnboardingItems { get; set; } = null!;

    /// <summary>
    /// Gets or sets the onboarding templates.
    /// </summary>
    public DbSet<OnboardingTemplate> OnboardingTemplates { get; set; } = null!;

    /// <summary>
    /// Gets or sets the items within onboarding templates.
    /// </summary>
    public DbSet<OnboardingTemplateItem> OnboardingTemplateItems { get; set; } = null!;

    /// <summary>
    /// Gets or sets the offboarding checklists.
    /// </summary>
    public DbSet<OffboardingChecklist> OffboardingChecklists { get; set; } = null!;

    /// <summary>
    /// Gets or sets the individual offboarding tasks.
    /// </summary>
    public DbSet<OffboardingTask> OffboardingTasks { get; set; } = null!;

    /// <summary>
    /// Gets or sets the exit interviews.
    /// </summary>
    public DbSet<ExitInterview> ExitInterviews { get; set; } = null!;

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
        modelBuilder.ApplyConfiguration(new OnboardingChecklistConfiguration());
        modelBuilder.ApplyConfiguration(new OnboardingItemConfiguration());
        modelBuilder.ApplyConfiguration(new OnboardingTemplateConfiguration());
        modelBuilder.ApplyConfiguration(new OnboardingTemplateItemConfiguration());
        modelBuilder.ApplyConfiguration(new OffboardingChecklistConfiguration());
        modelBuilder.ApplyConfiguration(new OffboardingTaskConfiguration());
        modelBuilder.ApplyConfiguration(new ExitInterviewConfiguration());

        // MassTransit Outbox configuration
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        // Apply snake_case naming last to catch all configurations
        SnakeCaseNamingHelper.ApplySnakeCaseNaming(modelBuilder);
    }
}
