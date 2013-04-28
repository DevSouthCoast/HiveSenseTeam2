using System;
using System.Collections;
using Gadgeteer.Modules.Seeed;
using HiveSense.Persistence;
using Microsoft.SPOT;

namespace HiveSense
{
    public class BarometerSensor
    {
        private readonly ILogger _logger;
        private readonly Barometer _sensor;

        public BarometerSensor(ILogger logger, Barometer sensor)
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
            var values = new Hashtable();
            values.Add("Pressure", sensorData.Pressure);
            values.Add("Temperature", sensorData.Temperature);
            _logger.Log("Barometer", values);
        }
    }
}
