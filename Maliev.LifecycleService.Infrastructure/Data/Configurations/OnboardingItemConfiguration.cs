using Maliev.LifecycleService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.LifecycleService.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the <see cref="OnboardingItem"/> entity.
/// </summary>
public class OnboardingItemConfiguration : IEntityTypeConfiguration<OnboardingItem>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<OnboardingItem> builder)
    {
        builder.ToTable("onboarding_items");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Category).IsRequired();
        builder.Property(x => x.CreatedDate).IsRequired();

        builder.HasIndex(x => x.OnboardingChecklistId);
        builder.HasIndex(x => x.AssignedTo);
    }
}