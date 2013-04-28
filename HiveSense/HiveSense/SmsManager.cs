using System;
using Microsoft.SPOT;
using GT = Gadgeteer;
using Gadgeteer.Modules.Seeed;
using System.Threading;
using System.Collections;

namespace HiveSense
{
    /// <summary>
    /// Component to send SMS messages on demand.
    /// </summary>
    class SmsManager
    {
        private const int StartUpDelaySec = 40;
        private readonly CellularRadio cellularRadio;
        private readonly string alertNumber;

        private readonly IList messageList = new ArrayList();
        private bool isSending;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellularRadio"></param>
        /// <param name="alertNumber"></param>
        public SmsManager(CellularRadio cellularRadio, string alertNumber)
        {
            this.cellularRadio = cellularRadio;
            this.alertNumber = alertNumber;
            cellularRadio.DebugPrintEnabled = true;
        } 

   


        /// <summary>
        /// Send an SMS message.
        /// 
        /// If not already sending start the phone, and start listening for send events.
        /// If already in the process of sending, just add to the queue.
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(string message)
        {
            lock (messageList)
            {
                messageList.Add(message);
                if (!isSending)
                {
                    isSending = true;
                    Debug.Print("Turning on cellular radio");
                    cellularRadio.PowerOn(StartUpDelaySec);
                    cellularRadio.ModuleInitialized += new CellularRadio.ModuleInitializedHandler(cellularRadio_ModuleInitialized_Send);
                }
            }
        }

        /// <summary>
        /// When the module is initialised, 
        /// </summary>
        /// <param name="sender"></param>
        void cellularRadio_ModuleInitialized_Send(CellularRadio sender)
        {
            lock (messageList)
            {
                Debug.Print("Sending SMS: count = " + messageList.Count);
                foreach (object message in messageList)
                {
                    var sendResult = cellularRadio.SendSms(alertNumber, (string)message);
                    Debug.Print("Result = " + sendResult.ToString());
                }
                messageList.Clear();
                cellularRadio.PowerOff();
                isSending = false;
            }
        }



        /// <summary>
        /// Get some general debugging information
        /// </summary>
        public void DebugStatus()
        {
            Debug.Print("Turning on cellular radio");
            cellularRadio.PowerOn(StartUpDelaySec);

            cellularRadio.ModuleInitialized += new CellularRadio.ModuleInitializedHandler(cellularRadio_ModuleInitialized_Debug);
        }

        void cellularRadio_ModuleInitialized_Debug(CellularRadio sender)
        {
            Debug.Print("Module Initialised");

            cellularRadio.ImeiRetrieved += new CellularRadio.ImeiRetrievedHandler(cellularRadio_ImeiRetrieved_Debug);

            var imeiReturn = cellularRadio.RetrieveImei();
            Debug.Print("IMEI return: " + imeiReturn);

            cellularRadio.SignalStrengthRetrieved += new CellularRadio.SignalStrengthRetrievedHandler(cellularRadio_SignalStrengthRetrieved_Debug);
            var ssReturn = cellularRadio.RetrieveSignalStrength();
            Debug.Print("Signal strength return: " + ssReturn);

            cellularRadio.OperatorRetrieved += new CellularRadio.OperatorRetrievedHandler(cellularRadio_OperatorRetrieved_Debug);
            var roReturn = cellularRadio.RetrieveOperator();
            Debug.Print("Operator return: " + roReturn);


            cellularRadio.PhoneActivityRetrieved += new CellularRadio.PhoneActivityRetrievedHandler(cellularRadio_PhoneActivityRetrieved_Debug);
            var paReturn = cellularRadio.RetrievePhoneActivity();
            Debug.Print("Phone activity return: " + paReturn);
        }

        void cellularRadio_ImeiRetrieved_Debug(CellularRadio sender, string imei)
        {
            Debug.Print("IMEI retrieved: " + imei);
        }

        void cellularRadio_SignalStrengthRetrieved_Debug(CellularRadio sender, CellularRadio.SignalStrengthType signalStrength)
        {
            Debug.Print("Signal strength: " + signalStrength.ToString());
        }

        void cellularRadio_OperatorRetrieved_Debug(CellularRadio sender, string operatorName)
        {
            Debug.Print("Operator retrieved: " + operatorName);
        }

        void cellularRadio_PhoneActivityRetrieved_Debug(CellularRadio sender, CellularRadio.PhoneActivityType activity)
        {
            Debug.Print("Phone activity: " + activity);
        }
    }
}
