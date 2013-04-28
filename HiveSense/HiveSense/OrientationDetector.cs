using System;
using Microsoft.SPOT;
using Gadgeteer.Modules.Seeed;
using GT = Gadgeteer;

namespace HiveSense
{
    /// <summary>
    /// Detect orientation in the accelerometer.
    /// If falls from vertical, send an alert.
    /// 
    /// Once alerted, reset once reaches vertical.
    /// 
    /// This current assumes that the accelerometer is positioned so that the y axis
    /// points straight down - resting state for acceleration should be (0, -1, 0).
    /// Z might seem more sensible, but the accelerometer is to be mounted on 
    /// a vertical board.
    /// 
    /// Doesn't start checking until initially put into a vertical position.
    /// 
    /// </summary>
    class OrientationDetector
    {
        private const double DetectVerticalTolerance = 0.2;
        private const double DetectFallTolerance = 0.3;

        private readonly Accelerometer accelerometer;
        private readonly INotifier notifier;
        private bool isMonitoring;  // true if currently checking for fall
        private bool isInitialised; // set to true the first time it's vertical

        public OrientationDetector(Accelerometer accelerometer, INotifier notifier)
        { 
            this.accelerometer = accelerometer;
            this.notifier = notifier;
            accelerometer.MeasurementComplete +=new Accelerometer.MeasurementCompleteEventHandler(accelerometer_MeasurementComplete);
        }

        
        /// <summary>
        /// Start listening for events
        /// </summary>
        public void Start() 
        {
            isInitialised = false;
            var timer = new GT.Timer(2000);
            timer.Tick += new GT.Timer.TickEventHandler(timer_Tick);
            timer.Start();
        }

        /// <summary>
        /// Periodically request a measurement
        /// </summary>
        /// <param name="timer"></param>
        void timer_Tick(GT.Timer timer)
        {
            accelerometer.RequestMeasurement();
        }

        /// <summary>
        /// Respond to a periodic measurement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="acceleration"></param>
        void accelerometer_MeasurementComplete(Accelerometer sender, Accelerometer.Acceleration acceleration)
        {
            if (!isInitialised)
            {
                if (IsCloseToVertical(acceleration))
                {
                    Debug.Print("Is vertical, will start monitoring: acceleration = " + acceleration);
                    isInitialised = true;
                    isMonitoring = true;
                }
                else
                {
                    Debug.Print("Is not vertical, will not start monitoring yet: acceleration = " + acceleration);
                }
            }
            else
            {
                var stateDescription = isMonitoring
                    ? "Checking for away from vertical"
                    : "Waiting to be returned to vertical";
                Debug.Print(stateDescription + ": acceleration = (" + acceleration.X + "," + acceleration.Y + "," + acceleration.Z + ")\n");

                if (isMonitoring)
                {
                    if (IsTooFarFromVertical(acceleration))
                    {
                        Debug.Print("Hive fall detected");
                        notifier.SendNotification("The hive has fallen over!!!");
                        isMonitoring = false;
                    }
                }
                else
                {
                    if (IsCloseToVertical(acceleration))
                    {
                        Debug.Print("Hive back the right way - start monitoring again");
                        isMonitoring = true;
                    }
                }
            }
        }


        /// <summary>
        /// Return true if too far from vertical, so should send an alert.
        /// </summary>
        /// <returns></returns>
        bool IsTooFarFromVertical(Accelerometer.Acceleration acceleration)
        {
            return (System.Math.Abs(acceleration.X) > DetectFallTolerance) 
                || (System.Math.Abs(acceleration.Z) > DetectFallTolerance);
        }

        /// <summary>
        /// Returns true if close enough to vertical to start monitoring again.
        /// This should be more sensitive than IsTooFarFromVertical, so that
        /// this won't turn on and immediately send a new value.
        /// </summary>
        /// <returns></returns>
        bool IsCloseToVertical(Accelerometer.Acceleration acceleration)
        {
            return (System.Math.Abs(acceleration.X) < DetectVerticalTolerance)
                && (System.Math.Abs(acceleration.Z) < DetectVerticalTolerance);
        }





    }
}
