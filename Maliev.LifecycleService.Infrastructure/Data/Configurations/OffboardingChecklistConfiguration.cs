using Maliev.LifecycleService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.LifecycleService.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the <see cref="OffboardingChecklist"/> entity.
/// </summary>
public class OffboardingChecklistConfiguration : IEntityTypeConfiguration<OffboardingChecklist>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<OffboardingChecklist> builder)
    {
        builder.ToTable("offboarding_checklists");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.EmployeeId).IsRequired();
        builder.HasIndex(x => x.EmployeeId).IsUnique();

        builder.Property(x => x.Status).IsRequired();
        builder.HasIndex(x => x.Status);

        builder.Property(x => x.TerminationDate).IsRequired();
        builder.Property(x => x.TerminationReason).IsRequired();
        builder.Property(x => x.CreatedDate).IsRequired();

        builder.HasMany(x => x.Tasks)
            .WithOne(x => x.Checklist)
            .HasForeignKey(x => x.OffboardingChecklistId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}