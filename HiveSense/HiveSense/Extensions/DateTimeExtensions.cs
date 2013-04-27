using System;
using Microsoft.SPOT;

namespace HiveSense.Extensions
{
    public static class DateTimeExtensions
    {
        public static long ToUnixTicks(this DateTime dt)
        {
            DateTime epoch = new DateTime(1970, 1, 1);
            DateTime d2 = dt.ToUniversalTime();
            TimeSpan ts = new TimeSpan(d2.Ticks - epoch.Ticks);
            return ts.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public static DateTime FromUnixTicks(long unixTicks)
        {
            DateTime epoch = new DateTime(1970, 1, 1);
            return epoch.AddTicks(unixTicks);
        }
    }
}
