using Maliev.LifecycleService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.LifecycleService.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the <see cref="ExitInterview"/> entity.
/// </summary>
public class ExitInterviewConfiguration : IEntityTypeConfiguration<ExitInterview>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<ExitInterview> builder)
    {
        builder.ToTable("exit_interviews");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.ConductedBy).IsRequired();
        builder.Property(x => x.InterviewDate).IsRequired();
        builder.Property(x => x.CreatedDate).IsRequired();

        builder.HasOne(x => x.OffboardingChecklist)
            .WithOne()
            .HasForeignKey<ExitInterview>(x => x.OffboardingChecklistId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
