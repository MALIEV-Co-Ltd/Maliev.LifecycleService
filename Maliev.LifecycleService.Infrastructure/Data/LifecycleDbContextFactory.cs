using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Maliev.LifecycleService.Infrastructure.Data;

/// <summary>
/// Design-time factory for the <see cref="LifecycleDbContext"/>.
/// Used by Entity Framework Core tools for migrations.
/// </summary>
public class LifecycleDbContextFactory : IDesignTimeDbContextFactory<LifecycleDbContext>
{
    /// <inheritdoc/>
    public LifecycleDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LifecycleDbContext>();

        // For design-time (migrations), prefer environment variable to avoid hardcoded secrets
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__LifecycleDbContext")
            ?? "Host=localhost;Database=lifecycledb;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString,
            x => x.MigrationsHistoryTable("__EFMigrationsHistory", "public"));

        return new LifecycleDbContext(optionsBuilder.Options);
    }
}