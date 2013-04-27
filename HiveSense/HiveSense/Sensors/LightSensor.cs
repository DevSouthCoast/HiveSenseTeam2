using Gadgeteer;
using HiveSense.Persistence;
using Microsoft.SPOT;

namespace HiveSense.Sensors
{
    /// <summary>
    /// The LightSensor class captures the readings from the Light Sensor and
    /// passes them to the Logger object.
    /// </summary>
    public class LightSensor
    {
        private readonly ILogger _logger;
        private readonly Gadgeteer.Modules.GHIElectronics.LightSensor _sensor;

        /// <summary>
        /// Initializes a new instance of the <see cref="LightSensor"/> class.
        /// </summary>
        /// <param name="logger">The logger object that the sensor readings are being logged to.</param>
        /// <param name="sensor">The sensor that is being monitored.</param>
        /// <param name="readingIntervalSeconds">The interval at which the sensor should be read and logged (in seconds)</param>
        public LightSensor(ILogger logger, Gadgeteer.Modules.GHIElectronics.LightSensor sensor, int readingIntervalSeconds)
        {
            // Store the parameters for use by the class.
            _logger = logger;
            _sensor = sensor;

            // Initialise a timer at the given interval seconds.
            var timer = new Timer(readingIntervalSeconds * 1000);
            timer.Tick += TimerOnTick;

            // Start the timer.
            timer.Start();
        }

        /// <summary>
        /// Handles the tick event of the internal timer that operates the light sensor reading
        /// interval.
        /// </summary>
        /// <param name="timer">The timer.</param>
        private void TimerOnTick(Timer timer)
        {
            // Stop the timer.
            timer.Stop();
            try
            {
                // Read and log the sensor.
                var lsp = _sensor.ReadLightSensorPercentage();
                _logger.Log("Light", lsp);
            }
            finally
            {
                // Re-start the timer again.
                timer.Start();
            }
        }
    }
}
