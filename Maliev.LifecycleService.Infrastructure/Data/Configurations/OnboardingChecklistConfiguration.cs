using Maliev.LifecycleService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.LifecycleService.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the <see cref="OnboardingChecklist"/> entity.
/// </summary>
public class OnboardingChecklistConfiguration : IEntityTypeConfiguration<OnboardingChecklist>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<OnboardingChecklist> builder)
    {
        builder.ToTable("onboarding_checklists");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.EmployeeId).IsRequired();
        builder.HasIndex(x => x.EmployeeId).IsUnique();

        builder.Property(x => x.Status).IsRequired();
        builder.HasIndex(x => x.Status);

        builder.Property(x => x.StartDate).IsRequired();
        builder.Property(x => x.CreatedDate).IsRequired();

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Checklist)
            .HasForeignKey(x => x.OnboardingChecklistId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}