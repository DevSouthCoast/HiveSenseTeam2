using Gadgeteer.Modules.Seeed;
using Microsoft.SPOT;

namespace HiveSense
{
    /// <summary>
    /// The TemperatureHumiditySensor class captures the readings from the Temperature and Humidity Sensor and
    /// passes them to the Logger object.
    /// </summary>
    public class TemperatureHumiditySensor
    {
        private readonly Logger _logger;
        private readonly TemperatureHumidity _sensor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemperatureHumiditySensor"/> class.
        /// </summary>
        /// <param name="logger">The logger that the sensor readings are being sent to.</param>
        /// <param name="sensor">The sensor that is being logged.</param>
        public TemperatureHumiditySensor(Logger logger, TemperatureHumidity sensor)
        {
            // Store the logger and sensor objects.
            _logger = logger;
            _sensor = sensor;

            // Put the sensor into continuous monitoring mode.
            _sensor.MeasurementComplete += SensorOnMeasurementComplete;
            _sensor.StartContinuousMeasurements();
        }

        /// <summary>
        /// Handles the receiving of measurements from the sensor.
        /// </summary>
        /// <param name="sender">The sensor that the measurements are from.</param>
        /// <param name="temperature">The temperature.</param>
        /// <param name="relativeHumidity">The relative humidity.</param>
        private void SensorOnMeasurementComplete(TemperatureHumidity sender, double temperature, double relativeHumidity)
        {
            // Log the readings.
            _logger.Log("Temperature", temperature);
            Debug.Print("Temperature:" + temperature);
            _logger.Log("Humidity", relativeHumidity);
            Debug.Print("Humidity:" + relativeHumidity);
        }
    }
}
