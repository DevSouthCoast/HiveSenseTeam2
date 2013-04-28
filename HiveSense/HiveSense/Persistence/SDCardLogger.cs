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
        private const long MaxFileSize = 5000;
        private SDCard card;
        private IDateTimeProvider dateTimeProvider;
        private bool noCard = true;

        // Dictionary<string, List<LogMessage>>
        private Hashtable memoryMessages = new Hashtable();
        private Hashtable logFileNames = new Hashtable();

        public SDCardLogger(IDateTimeProvider dateTimeProvider, SDCard card)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.card = card;
            
          //  this.card.SDCardMounted += new SDCard.SDCardMountedEventHandler(SDCardMounted);
            if (this.card.IsCardInserted)
            {
                this.card.MountSDCard();
            }
            UpdateLogIndexes();
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
            if (card.IsCardMounted)
            {
                StorageDevice device = card.GetStorageDevice();
                using (FileStream fileStream =
                    device.Open(GetFilePath(key), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fileStream.Seek(0, SeekOrigin.End);
                    using (var s = new StreamWriter(fileStream))
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
            UpdateLogIndexes();
        }

        private string GetFilePath(string key)
        {
            return GetFilePath(key, GetLogFileIndex(key));
        }

        private string GetFilePath(string key, int index)
        {
            return string.Concat("log-", key, "-", index);
        }

        private int GetLogFileIndex(string key)
        {
            var index = 0;
            if (logFileNames.ContainsKey(key))
            {
                index =  int.Parse(logFileNames[key].ToString());
            }

            StorageDevice device = card.GetStorageDevice();
            using (FileStream fileStream =
                device.Open(GetFilePath(key, index), FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (fileStream.Length > MaxFileSize)
                {
                    logFileNames[key] = ++index;
                   
                }
            }

            return index;
        }

        private void UpdateLogIndexes()
        {
            string[] files = card.GetStorageDevice().ListFiles("");
            foreach (var file in files)
            {
                if (file.IndexOf("log-") == 0)
                {
                    var actualName = file.Substring(4);
                    var indexerPosition = actualName.IndexOf("-");
                    var index = int.Parse(actualName.Substring(indexerPosition + 1));
                    actualName = actualName.Substring(0, indexerPosition);
                    if (logFileNames.ContainsKey(actualName))
                    {
                        var currentIndex = int.Parse(logFileNames[actualName].ToString());
                        if (index > currentIndex)
                        {
                            logFileNames[actualName] = index;
                        }
                    }
                    else
                    {
                        logFileNames.Add(actualName, index);
                    }
                }
            }
        }

        public bool CanRead()
        {
            return true;
        }

        /* List<LogMessage> */
        public LogMessage GetLastMessage(string key)
        { 
            string message = string.Empty;

            StorageDevice device = card.GetStorageDevice();
            using (FileStream fileStream =
                device.Open(GetFilePath(key), FileMode.OpenOrCreate, FileAccess.Read))
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                using (var s = new StreamReader(fileStream))
                {
                    if (s.Peek() != -1)
                    {
                        message = s.ReadLine();
                    }
                }
            }
            return LogMessage.FromLogFileFormat(message);
        }

        public ArrayList GetMessages(string key, int amount)
        {
            return GetMessages(key, amount, -99);
        }

        /* List<LogMessage> */
        public ArrayList GetMessages(string key, int amount, int index)
        {
            ArrayList ret = null;
            if (index != -1)
            {
                if(index < 0)
                {
                    index = GetLogFileIndex(key);
                }
                ret = new ArrayList();
                Stack messages = new Stack();

                StorageDevice device = card.GetStorageDevice();
                using (FileStream fileStream =
                    device.Open(GetFilePath(key, index), FileMode.OpenOrCreate,
                                FileAccess.Read))
                {
                    fileStream.Seek(0, SeekOrigin.Begin);
                    using (var s = new StreamReader(fileStream))
                    {
                        while (s.Peek() != -1)
                        {
                            string line = s.ReadLine();
                            messages.Push(LogMessage.FromLogFileFormat(line));
                        }
                    }
                }

                for (var i = messages.Count - 1; i >= 0 && ret.Count < amount; i--)
                {
                    ret.Add(messages.Pop());
                }

                if (ret.Count < amount)
                {
                    // Recurse
                    var olderData = GetMessages(key, amount-ret.Count /* The diff needed */, --index);
                    if (olderData != null)
                    {
                        return ret.AddRange(olderData);
                    }
                }
            }
            return ret;
        }

        public void Dispose()
        {
            this.card.UnmountSDCard();
        }
    }
}
