using System;
using System.Collections;
using System.Text;
using HiveSense.Extensions;

namespace HiveSense.Persistence
{
    public class LogMessage
    {
        public string Key { get; set; }
        public DateTime DateTime { get; set; }
        public Hashtable Values { get; set; }

        public LogMessage(DateTime dateTime, string key, Hashtable values)
        {
            this.DateTime = dateTime;
            this.Key = key;
            this.Values = values;
        }

        public string ToLogFileString()
        {
            var msg = new StringBuilder();
            msg.Append("{");
            msg.Append("\"key\":\"");
            msg.Append(this.Key);
            msg.Append("\",");
            msg.Append("\"time\":\"");
            msg.Append(this.DateTime.ToUnixTicks());
            msg.Append("\",");
            msg.Append("\"values\":{");
            var numberOfValues = this.Values.Keys.Count;
            var currentVal = 0;
            foreach(DictionaryEntry entry in this.Values)
            {
                msg.Append("\"" + entry.Key.ToString() + "\":\"");
                msg.Append(entry.Value.ToString());
                msg.Append("\"");
                if (currentVal < (numberOfValues - 1))
                {
                    msg.Append(",");    
                }
                currentVal++;
            }
            msg.Append("}");
            msg.Append("}");

            return msg.ToString();
        }
    }
}