using System;
using System.Collections;

namespace HiveSense.Persistence
{
    public interface ILogger : IDisposable
    {
        void Log(string key, object value);
        void Log(string key, Hashtable values);
        bool CanRead();
        LogMessage GetLastMessage(string key);
        ArrayList GetMessages(string key, int amount); /* List<LogMessage> */
    }
}