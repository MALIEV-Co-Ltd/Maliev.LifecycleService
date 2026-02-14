using Maliev.LifecycleService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Maliev.LifecycleService.Tests.Infrastructure;

/// <summary>Integrity tests.</summary>
public class ModelIntegrityTests
{
    /// <summary>Check for pending migrations.</summary>
    [Fact]
    public void Model_ShouldNotHavePendingChanges()
    {
        var options = new DbContextOptionsBuilder<LifecycleDbContext>()
            .UseNpgsql("Host=localhost;Database=ModelCheck")
            .Options;

        using var context = new LifecycleDbContext(options);
        var hasChanges = context.Database.HasPendingModelChanges();

        Assert.False(hasChanges, "Run 'dotnet ef migrations add <Name> --project Maliev.LifecycleService.Infrastructure --startup-project Maliev.LifecycleService.Api'");
    }
}
