namespace Bitbucket.Net.Common;

public static class UnixDateTimeExtensions
{
    public static DateTimeOffset FromUnixTimeMilliseconds(this long value)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(value)
            .ToLocalTime();
    }

    public static long ToUnixTimeMilliseconds(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToUnixTimeMilliseconds();
    }
}