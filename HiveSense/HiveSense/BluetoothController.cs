using System;
using Gadgeteer.Modules.GHIElectronics;
using Microsoft.SPOT;

namespace HiveSense
{
    public class BluetoothController
    {
        private readonly Bluetooth _bluetooth;
        private BarometerSensor _barometerSensor;
        private LightSensor _lightSensor;
        private TemperatureHumiditySensor _temperatureHumiditySensor;

        public BluetoothController(Bluetooth bluetooth, BarometerSensor barometerSensor, LightSensor lightSensor, TemperatureHumiditySensor temperatureHumiditySensor, string deviceName, string pin)
        {
            _bluetooth = bluetooth;

            _bluetooth.SetDeviceName(deviceName);
            _bluetooth.SetPinCode(pin);
            _bluetooth.BluetoothStateChanged += BluetoothOnBluetoothStateChanged;
            _bluetooth.DataReceived += _bluetooth_DataReceived;

            _barometerSensor = barometerSensor;
            _lightSensor = lightSensor;
            _temperatureHumiditySensor = temperatureHumiditySensor;
        }

        void _bluetooth_DataReceived(Bluetooth sender, string data)
        {
            if (data.ToLower() == "getmetrics")
            {
                _bluetooth.ClientMode.SendLine("Temperature|" + _temperatureHumiditySensor.LastTemperature().ToString());
                _bluetooth.ClientMode.SendLine("RelativeHumidity|" + _temperatureHumiditySensor.LastRelativeHumidity().ToString());
                _bluetooth.ClientMode.SendLine("BarometricPressure|" + _barometerSensor.LastReading().Pressure);
                _bluetooth.ClientMode.SendLine("BarometerTemperature|" + _barometerSensor.LastReading().Temperature);
                _bluetooth.ClientMode.SendLine("LightLevel|" + _lightSensor.LastReading().ToString());
            }
        }

        private void BluetoothOnBluetoothStateChanged(Bluetooth sender, Bluetooth.BluetoothState btState)
        {
        }

        public void StartPairing()
        {
            _bluetooth.ClientMode.EnterPairingMode();
        }       
    }
}
