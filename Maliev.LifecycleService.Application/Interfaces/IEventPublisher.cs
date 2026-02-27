namespace Maliev.LifecycleService.Application.Interfaces;

/// <summary>
/// Defines the service for publishing events to the message bus.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes an event of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the event message.</typeparam>
    /// <param name="message">The event message to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
}
