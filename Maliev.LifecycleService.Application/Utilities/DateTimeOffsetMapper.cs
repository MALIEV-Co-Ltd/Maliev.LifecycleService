namespace Maliev.LifecycleService.Application.Utilities;

/// <summary>
/// Maps lifecycle date-only values to UTC offsets for integration event contracts.
/// </summary>
public static class DateTimeOffsetMapper
{
    /// <summary>
    /// Converts a lifecycle date to a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="value">The date value to map.</param>
    /// <returns>A UTC date-time offset.</returns>
    public static DateTimeOffset FromUtcDateTime(DateTime value)
    {
        if (value == default)
        {
            return default;
        }

        var utcValue = value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

        return new DateTimeOffset(utcValue);
    }
}
