using System;

namespace HiveSense.Persistence
{
    public interface IDateTimeProvider
    {
        DateTime GetDateTime();
    }
}