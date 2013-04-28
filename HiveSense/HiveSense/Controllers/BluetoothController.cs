using Gadgeteer.Modules.GHIElectronics;
using HiveSense.Persistence;
using HiveSense.Sensors;
using LightSensor = Gadgeteer.Modules.GHIElectronics.LightSensor;

namespace HiveSense.Controllers
{
    public class BluetoothController
    {
        private readonly Bluetooth _bluetooth;
        private BarometerSensor _barometerSensor;
        private Sensors.LightSensor _lightSensor;
        private TemperatureHumiditySensor _temperatureHumiditySensor;
        private ILogger _logger;

        public BluetoothController(ILogger logger, Bluetooth bluetooth, BarometerSensor barometerSensor, Sensors.LightSensor lightSensor, TemperatureHumiditySensor temperatureHumiditySensor, string deviceName, string pin)
        {
            _logger = logger;
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
                // Send latest readings
                _bluetooth.ClientMode.SendLine(_logger.GetLastMessage("TempHumid").ToLogFileString());
                _bluetooth.ClientMode.SendLine(_logger.GetLastMessage("Barometer").ToLogFileString());
                _bluetooth.ClientMode.SendLine(_logger.GetLastMessage("Light").ToLogFileString());
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
