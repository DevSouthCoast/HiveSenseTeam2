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
            msg.Append(this.DateTime.ToUnixMilliseconds());
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

        public static LogMessage FromLogFileFormat(string logMsg)
        {
            //{"key":"TempHumid","time":"1306886422750","values":{"Temperature":"22.670000000000002","Humidity":"40.039999999999999"}}
            var builder = new StringBuilder(logMsg);
            builder.Replace("\"", string.Empty);
            builder.Replace("{", string.Empty);
            builder.Replace("}", string.Empty);

            string[] values = builder.ToString().Split(new char[] { ':', ',' });

            var log = new LogMessage(DateTime.UtcNow, null, null);
            log.Key = values[1];
            log.DateTime = DateTimeExtensions.FromUnixTicks(long.Parse(values[3]));
            log.Values = new Hashtable();
            for(var i = 5; i+1 < values.Length;i+=2)
            {
                log.Values.Add(values[i], values[i+1]);
            }
            return log;
        }
    }
}