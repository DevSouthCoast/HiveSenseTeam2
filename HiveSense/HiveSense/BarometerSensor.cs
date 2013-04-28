using System;
using Gadgeteer.Modules.Seeed;
using Microsoft.SPOT;

namespace HiveSense
{
    public class BarometerSensor
    {
        private readonly Logger _logger;
        private readonly Barometer _sensor;
        private Barometer.SensorData _lastReadings;

        public BarometerSensor(Logger logger, Barometer sensor)
        {
            // Store the parameters for use by the class.
            _logger = logger;
            _sensor = sensor;

            _sensor.ContinuousMeasurementInterval = new TimeSpan(0, 0, 0, 3, 600);
            _sensor.MeasurementComplete += SensorOnMeasurementComplete;
            _sensor.StartContinuousMeasurements();
            
        }

        private void SensorOnMeasurementComplete(Barometer sender, Barometer.SensorData sensorData)
        {
            _lastReadings = sensorData;
            _logger.Log("BarometerPressure", sensorData.Pressure);
            Debug.Print("BarometerPressure:" + sensorData.Pressure);
            _logger.Log("BarometerTemperature", sensorData.Temperature);
            Debug.Print("BarometerTemperature:" + sensorData.Temperature);
        }

        public Barometer.SensorData LastReading()
        {
            return _lastReadings;
        }
    }
}
