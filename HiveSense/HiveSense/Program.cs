using System;
using System.Collections;
using System.Threading;
using HiveSense.Controllers;
using HiveSense.Persistence;
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
        private ILogger _logger;
        private Sensors.TemperatureHumiditySensor _temperatureHumiditySensor;
        private Sensors.LightSensor _lightSensor;
        private BarometerSensor _barometer;
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
            var sdlogger = new SDCardLogger(new RtcDateTimeProvider(), sdCard);
            var consolelogger = new ConsoleLogger(new RtcDateTimeProvider());
            _logger = new AggregateLogger(new ArrayList() {sdlogger, consolelogger});

            _temperatureHumiditySensor = new Sensors.TemperatureHumiditySensor(_logger, temperatureHumidity);
            _lightSensor = new Sensors.LightSensor(_logger, lightSensor, 15);
            _barometer = new BarometerSensor(_logger, barometer);
            _bluetoothController = new BluetoothController(_logger, bluetooth, _barometer, _lightSensor, _temperatureHumiditySensor, "HiveSense8765", "8765");
            _bluetoothController.StartPairing();
// 
            var smsManager = new SmsManager(cellularRadio, "YOUR PHONE NUMBER HERE");
            var smsNotifier = new SmsNotifier(smsManager);
            display_N18.Initialize(4000);
            var displayNotifier = new N18DisplayNotifier(display_N18);
            var debugNotifier = new DebugNotifier();
            var aggregateNotifier = new AggregateNotifier(displayNotifier, debugNotifier);

            //var impactDetector = new ImpactDetector(accelerometer, aggregateNotifier);
            //impactDetector.Start();

            var orientationDetector = new OrientationDetector(accelerometer, aggregateNotifier);
            orientationDetector.Start();
        }
    }
}
