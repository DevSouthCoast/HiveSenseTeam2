using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.Seeed;

namespace HiveSense
{
    public partial class Program
    {
        private Logger _logger;
        private TemperatureHumiditySensor _temperatureHumiditySensor;
        private LightSensor _lightSensor;
        private BarometerSensor _barometerSensor;
        private ButtonSensor _buttonSensor;
        private BluetoothController _bluetoothController;

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            /*******************************************************************************************
            Modules added in the Program.gadgeteer designer view are used by typing 
            their name followed by a period, e.g.  button.  or  camera.
            
            Many modules generate useful events. Type +=<tab><tab> to add a handler to an event, e.g.:
                button.ButtonPressed +=<tab><tab>
            
            If you want to do something periodically, use a GT.Timer and handle its Tick event, e.g.:
                GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
                timer.Tick +=<tab><tab>
                timer.Start();
            *******************************************************************************************/

            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.

            Debug.Print("Program Started");

            // Setup a dummy logger
            _logger = new Logger();
            Debug.Print("Logger created.");

            // Create the temperature and humidity sensor monitor.
            _temperatureHumiditySensor = new TemperatureHumiditySensor(_logger, temperatureHumidity);
            Debug.Print("Temperature and humidity monitoring started.");

            // Create the light sensor monitor.
            _lightSensor = new LightSensor(_logger, lightSensor, 15);
            Debug.Print("Light sensor monitoring started.");

            _barometerSensor = new BarometerSensor(_logger, barometer);
            Debug.Print("Barometer sensor monitoring started.");         

            _bluetoothController = new BluetoothController(bluetooth, _barometerSensor, _lightSensor, _temperatureHumiditySensor, "HiveSense9876", "9876");
            Debug.Print("Bluetooth controller started.");

            _buttonSensor = new ButtonSensor(button, _bluetoothController);
            Debug.Print("Button monitoring started.");
        }
    }
}
