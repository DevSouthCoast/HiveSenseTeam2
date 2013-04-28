using System;
using Microsoft.SPOT;
using Gadgeteer.Modules.GHIElectronics;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using GT = Gadgeteer;
using System.Threading;

namespace HiveSense
{
    /// <summary>
    /// Defines a component that can send notifications to something.
    /// </summary>
    interface INotifier
    {
        void SendNotification(string message);
    }

    /// <summary>
    /// Notifier that combines multiple notifiers and sends to all of them
    /// </summary>
    class AggregateNotifier : INotifier
    {
        private readonly INotifier[] notifiers;

        public AggregateNotifier(params INotifier[] notifiers)
        {
            this.notifiers = notifiers;
        }
        public void SendNotification(string message)
        {
            foreach (var notifier in notifiers)
            {
                try
                {
                    notifier.SendNotification(message);
                }
                catch (Exception e)
                {
                    Debug.Print(e.Message + "\n" + e.StackTrace);
                }            

            }
        }
    }

    /// <summary>
    /// Notifier that just uses debug output
    /// </summary>
    class DebugNotifier : INotifier
    {
        public void SendNotification(string message)
        {
            Debug.Print("ALERT: " + message);
        }
    }

    /// <summary>
    /// Notifier that sends an SMS message
    /// </summary>
    class SmsNotifier : INotifier
    {
        private readonly SmsManager smsManager;

        public SmsNotifier(SmsManager smsManager)
        {
            this.smsManager = smsManager;
        }


        public void SendNotification(string message)
        {
            smsManager.SendMessage(message);
        }
    }

    /// <summary>
    /// Notifier that writes to N18 display
    /// </summary>
    class N18DisplayNotifier : INotifier
    {
        private readonly Display_N18 display;

        public N18DisplayNotifier(Display_N18 display)
        {
            this.display = display;
        }

        public void SendNotification(string message)
        {
            display.SimpleGraphics.DisplayTextInRectangle(message,
                0, 0, 100, 100, GT.Color.Red, Resources.GetFont(Resources.FontResources.NinaB),
                GT.Modules.Module.DisplayModule.SimpleGraphicsInterface.TextAlign.Center);
           

            new Thread(new ThreadStart(() => {
                Thread.Sleep(5000);
                display.Reset();
            })).Start();
        }
    }
}
 