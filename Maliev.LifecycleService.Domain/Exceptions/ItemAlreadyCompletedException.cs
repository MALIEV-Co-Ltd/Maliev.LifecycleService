namespace Maliev.LifecycleService.Domain.Exceptions;

/// <summary>
/// Exception thrown when attempting to complete an already-completed task (concurrent modification scenario).
/// First completion wins; second attempt receives this error.
/// </summary>
public class ItemAlreadyCompletedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemAlreadyCompletedException"/> class.
    /// </summary>
    /// <param name="itemId">The identifier of the item that was already completed.</param>
    public ItemAlreadyCompletedException(Guid itemId)
        : base($"Item {itemId} is already completed. First completion wins in concurrent scenarios.")
    {
        ItemId = itemId;
    }

    /// <summary>
    /// Gets the identifier of the item that was already completed.
    /// </summary>
    public Guid ItemId { get; }
}
