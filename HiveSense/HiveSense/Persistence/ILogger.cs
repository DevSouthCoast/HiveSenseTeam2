using System;
using System.Collections;

namespace HiveSense.Persistence
{
    public interface ILogger : IDisposable
    {
        void Log(string key, object value);
        void Log(string key, Hashtable values);
    }
}