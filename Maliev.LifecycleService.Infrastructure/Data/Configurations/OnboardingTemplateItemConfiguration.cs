using Maliev.LifecycleService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.LifecycleService.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the <see cref="OnboardingTemplateItem"/> entity.
/// </summary>
public class OnboardingTemplateItemConfiguration : IEntityTypeConfiguration<OnboardingTemplateItem>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<OnboardingTemplateItem> builder)
    {
        builder.ToTable("onboarding_template_items");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Category).IsRequired();
        builder.Property(x => x.DefaultAssigneeRole).HasMaxLength(50);
    }
}