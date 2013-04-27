using System;
using System.Collections;

namespace HiveSense.Persistence
{
    public interface ILogger
    {
        void Log(string key, object value);
        void Log(string key, Hashtable values);
    }
}