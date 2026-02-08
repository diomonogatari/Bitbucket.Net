namespace Bitbucket.Net.Common;

/// <summary>
/// Extension methods for converting between Unix epoch milliseconds and <see cref="DateTimeOffset"/>.
/// Bitbucket Server represents all timestamps as milliseconds since the Unix epoch (1970-01-01T00:00:00Z).
/// </summary>
public static class UnixDateTimeExtensions
{
    /// <summary>
    /// Converts a Unix epoch millisecond timestamp to a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="value">The number of milliseconds since 1970-01-01T00:00:00Z.</param>
    /// <returns>A <see cref="DateTimeOffset"/> in UTC representing the given timestamp.</returns>
    public static DateTimeOffset FromUnixTimeMilliseconds(this long value)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(value);
    }

    /// <summary>
    /// Converts a <see cref="DateTimeOffset"/> to Unix epoch milliseconds.
    /// </summary>
    /// <param name="dateTimeOffset">The date and time to convert.</param>
    /// <returns>The number of milliseconds since 1970-01-01T00:00:00Z.</returns>
    public static long ToUnixTimeMilliseconds(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToUnixTimeMilliseconds();
    }
}