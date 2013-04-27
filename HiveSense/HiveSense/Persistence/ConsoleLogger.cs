using System;
using System.IO;
using Gadgeteer;
using HiveSense.Extensions;
using Microsoft.SPOT;
using Gadgeteer.Modules.GHIElectronics;
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

        public void Dispose()
        {
        }
    }
}
