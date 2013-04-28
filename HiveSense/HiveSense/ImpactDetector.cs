using System;
using Microsoft.SPOT;
using Gadgeteer.Modules.Seeed;
using GTM = Gadgeteer.Modules;
using GT = Gadgeteer;
using System.Threading;

namespace HiveSense
{
    /// <summary>
    /// Class for handling impact detection using an Accelerometer.
    /// 
    /// Detects acceleration beyond a threshold and fires an alert, 
    /// resetting itself 5 seconds later.
    /// 
    /// This is probably too sensitive.
    /// </summary>
    class ImpactDetector
    {
        private readonly Accelerometer accelerometer;
        private readonly INotifier notifier;

        /// <summary>
        /// Initialise with accelerometer and a notification target.
        /// </summary>
        /// <param name="accelerometer"></param>
        /// <param name="notifier"></param>
        public ImpactDetector(Accelerometer accelerometer, INotifier notifier)
        {
            this.accelerometer = accelerometer;
            this.notifier = notifier;

            accelerometer.LoadCalibration();
            accelerometer.MeasurementRange = GTM.Seeed.Accelerometer.Range.EightG;
            accelerometer.ThresholdExceeded += new Accelerometer.ThresholdExceededEventHandler(accelerometer_ThresholdExceeded);
            
        }

         
        /// <summary>
        /// Start listening to events.
        /// </summary>
        public void Start()
        {
            accelerometer.EnableThresholdDetection(7.0, true, true, true, true, false, false);

            // This bit is used 
            accelerometer.MeasurementComplete += new Accelerometer.MeasurementCompleteEventHandler(accelerometer_MeasurementComplete);
            var timer = new GT.Timer(5000);
            timer.Tick += new GT.Timer.TickEventHandler(timer_Tick);
            timer.Start();
        }

        /// <summary>
        /// Handle threshold exceeded event
        /// </summary>
        /// <param name="sender"></param>
        void accelerometer_ThresholdExceeded(Accelerometer sender)
        {
            // TODO work out how to identify this hive
            string message = "The hive has been disturbed: accelerometer threshold exceeded at "
                + DateTime.Now.ToString("HH:mm:ss, dd MMM yyyy");

            notifier.SendNotification(message);

            accelerometer.RequestMeasurement();

            // Wait a few seconds, then re-enable
            new Thread(new ThreadStart(() => { 
                Thread.Sleep(5000); 
                accelerometer.ResetThresholdDetection(); 
            })).Start();
        }


        void timer_Tick(GT.Timer timer)
        {
            accelerometer.RequestMeasurement();
        }


        void accelerometer_MeasurementComplete(Accelerometer sender, Accelerometer.Acceleration acceleration)
        {
            Debug.Print("current acceleration = (" + acceleration.X + "," + acceleration.Y + "," + acceleration.Z + ")\n");
        }
    }
}
