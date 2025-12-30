using Testcontainers.Redis;
using Xunit;

namespace Maliev.LifecycleService.Tests.Integration.Fixtures;

/// <summary>
/// XUnit fixture for setting up a Redis container for integration testing.
/// </summary>
public class RedisFixture : IAsyncLifetime
{
    private readonly RedisContainer _container = new RedisBuilder()
        .WithImage("redis:7-alpine")
        .Build();

    /// <summary>
    /// Gets the connection string for the Redis container.
    /// </summary>
    public string ConnectionString => _container.GetConnectionString();

    /// <summary>
    /// Initializes the Redis container.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    /// <summary>
    /// Stops and disposes of the Redis container.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DisposeAsync()
    {
        await _container.StopAsync();
    }
}