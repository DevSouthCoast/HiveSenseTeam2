using System.Collections;
using Gadgeteer.Modules.Seeed;
using HiveSense.Persistence;
using Microsoft.SPOT;

namespace HiveSense.Sensors
{
    /// <summary>
    /// The TemperatureHumiditySensor class captures the readings from the Temperature and Humidity Sensor and
    /// passes them to the Logger object.
    /// </summary>
    public class TemperatureHumiditySensor
    {
        private readonly ILogger _logger;
        private readonly TemperatureHumidity _sensor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemperatureHumiditySensor"/> class.
        /// </summary>
        /// <param name="logger">The logger that the sensor readings are being sent to.</param>
        /// <param name="sensor">The sensor that is being logged.</param>
        public TemperatureHumiditySensor(ILogger logger, TemperatureHumidity sensor)
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
            var values = new Hashtable();
            values.Add("Temperature", temperature);
            values.Add("Humidity", relativeHumidity);
            _logger.Log("TempHumid", values);
        }
    }
}
