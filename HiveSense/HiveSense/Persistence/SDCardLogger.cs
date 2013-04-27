using System;
using System.IO;
using Gadgeteer;
using HiveSense.Extensions;
using Microsoft.SPOT;
using Gadgeteer.Modules.GHIElectronics;
using System.Collections;

namespace HiveSense.Persistence
{
    public class SDCardLogger : ILogger
    {
        private const string SingleValueKey = "Value";
        private SDCard card;
        private IDateTimeProvider dateTimeProvider;
        private bool noCard = true;

        // Dictionary<string, List<LogMessage>>
        private Hashtable memoryMessages = new Hashtable();

        public SDCardLogger(IDateTimeProvider dateTimeProvider, SDCard card)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.card = card;
            
            if (this.card.IsCardInserted)
            {
                this.card.MountSDCard();
            }
            this.card.SDCardMounted += new SDCard.SDCardMountedEventHandler(SDCardMounted);
        }

        public void Log(string key, object value)
        {
            var values = new Hashtable { { SingleValueKey, value.ToString() } };
            this.Log(key, values);
        }

        public void Log(string key, Hashtable values)
        {
            if (noCard && this.card.IsCardInserted)
            {
                this.card.MountSDCard();
            }
            if (this.card.IsCardInserted && card.IsCardMounted)
            {
                LogToCard(this.dateTimeProvider.GetDateTime(), key, values);
            }
            else
            {
                LogToMemory(this.dateTimeProvider.GetDateTime(), key, values);
            }
        }

        private void LogToMemory(DateTime dateTime, string key, Hashtable values)
        {
            if(memoryMessages.ContainsKey(key))
            {
                var sensorQueue = memoryMessages[key] as ArrayList;
                if (sensorQueue != null)
                {
                    sensorQueue.Add(new LogMessage(dateTime, key, values));
                }
            }
        }

        private void LogToCard(DateTime dateTime, string key, Hashtable values)
        {
            if(card.IsCardMounted)
            {
                StorageDevice device = card.GetStorageDevice();
                using (FileStream fileStream = 
                    device.Open(key, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fileStream.Seek(0, SeekOrigin.End);
                    using(var s = new StreamWriter(fileStream))
                    {
                        s.WriteLine(new LogMessage(dateTime, key, values).ToLogFileString() + ",");
                    }
                }
            }
        }

        private void FlushMemoryMessages()
        {
            foreach (DictionaryEntry memoryMessage in memoryMessages)
            {
                var logMessages = memoryMessage.Value as ArrayList;
                if (logMessages != null)
                {
                    foreach (var message in logMessages)
                    {
                        var logMessage = message as LogMessage;
                        if (logMessage != null)
                        {
                            LogToCard(logMessage.DateTime, logMessage.Key, logMessage.Values);
                        }
                    }
                }
            }
            memoryMessages.Clear();
        }

        private void SDCardMounted(SDCard sender, Gadgeteer.StorageDevice sdCard)
        {
            noCard = false;
            FlushMemoryMessages();
        }

        public void Dispose()
        {
            this.card.UnmountSDCard();
        }
    }
}
