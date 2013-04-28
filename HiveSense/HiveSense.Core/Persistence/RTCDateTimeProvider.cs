using System;

namespace HiveSense.Persistence
{
    public class RtcDateTimeProvider : IDateTimeProvider
    {
        public DateTime GetDateTime()
        {
            return DateTime.UtcNow;
        }
    }
}