using System;
using System.IO;
using HiveSense.Extensions;
using Microsoft.SPOT;
using System.Collections;

namespace HiveSense.Persistence
{
    public class ConsoleLogger : ILogger
    {
        private const string SingleValueKey = "Value";
        private IDateTimeProvider dateTimeProvider;

        public ConsoleLogger(IDateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
        }

        public void Log(string key, object value)
        {
            var values = new Hashtable { { SingleValueKey, value.ToString() } };
            this.Log(key, values);
        }

        public void Log(string key, Hashtable values)
        {
            Debug.Print(new LogMessage(dateTimeProvider.GetDateTime(), key, values).ToLogFileString());
        }

        public bool CanRead()
        {
            return false;
        }

        /* List<LogMessage> */
        public LogMessage GetLastMessage(string key)
        {
            return null;
        }

        /* List<LogMessage> */
        public ArrayList GetMessages(string key, int amount)
        {
            return null;
        }

        public void Dispose()
        {
        }
    }
}
