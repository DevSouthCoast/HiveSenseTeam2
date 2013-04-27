using System.Collections;

namespace HiveSense.Persistence
{
    public class AggregateLogger : ILogger
    {
        private readonly ArrayList loggers;
        public AggregateLogger(ArrayList loggers)
        {
            this.loggers = loggers;
        }

        public void Log(string key, object value)
        {
            if(this.loggers != null)
            {
                foreach(ILogger logger in this.loggers)
                {
                    if (logger != null)
                    {
                        logger.Log(key, value);
                    }
                }
            }
        }

        public void Log(string key, Hashtable values)
        {
            if (this.loggers != null)
            {
                foreach (ILogger logger in this.loggers)
                {
                    if (logger != null)
                    {
                        logger.Log(key, values);
                    }
                }
            }
        }

        public void Dispose()
        {
            if (this.loggers != null)
            {
                foreach (ILogger logger in this.loggers)
                {
                    if (logger != null)
                    {
                        logger.Dispose();
                    }
                }
            }
        }
    }
}
