namespace HiveSense
{
    // This is a dummy logger class in lieu of the logger class being delivered
    // that writes to the SD card.
    public class Logger
    {
        public void Log(string logName, double value)
        {
            // Ignore the logged value.
        }
    }
}
