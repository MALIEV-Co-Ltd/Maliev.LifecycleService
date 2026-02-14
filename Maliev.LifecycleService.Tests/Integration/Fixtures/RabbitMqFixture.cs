using Testcontainers.RabbitMq;
using Xunit;

namespace Maliev.LifecycleService.Tests.Integration.Fixtures;

/// <summary>
/// XUnit fixture for setting up a RabbitMQ container for integration testing.
/// </summary>
public class RabbitMqFixture : IAsyncLifetime
{
    private readonly RabbitMqContainer _container = new RabbitMqBuilder().WithName("rabbitmq:3-management-alpine")
        .Build();

    /// <summary>
    /// Gets the connection string for the RabbitMQ container.
    /// </summary>
    public string ConnectionString => _container.GetConnectionString();

    /// <summary>
    /// Initializes the RabbitMQ container.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    /// <summary>
    /// Stops and disposes of the RabbitMQ container.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DisposeAsync()
    {
        await _container.StopAsync();
    }
}