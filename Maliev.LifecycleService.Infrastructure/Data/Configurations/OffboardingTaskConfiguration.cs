using Maliev.LifecycleService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.LifecycleService.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the <see cref="OffboardingTask"/> entity.
/// </summary>
public class OffboardingTaskConfiguration : IEntityTypeConfiguration<OffboardingTask>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<OffboardingTask> builder)
    {
        builder.ToTable("offboarding_tasks");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Category).IsRequired();
        builder.Property(x => x.CreatedDate).IsRequired();

        builder.HasIndex(x => x.OffboardingChecklistId);
    }
}