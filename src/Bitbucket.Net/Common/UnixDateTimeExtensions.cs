using System;

namespace Bitbucket.Net.Common;

public static class UnixDateTimeExtensions
{
    public static DateTimeOffset FromUnixTimeSeconds(this long value)
    {
        return DateTimeOffset.UnixEpoch.AddMilliseconds(value)
            .ToLocalTime();
    }

    public static long ToUnixTimeSeconds(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.Subtract(DateTimeOffset.UnixEpoch).Ticks;
    }
}