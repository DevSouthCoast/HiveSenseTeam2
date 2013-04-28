using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace PhoneApp
{
    public class HiveSenseCommunications
    {
        private StreamSocket socket;
        public StreamSocket Socket { get { return socket; } }

        private PeerInformation pi;

        public async void GetLatestMetrics(LatestMetricsDataModel dm)
        {
            socket = new StreamSocket();
            await socket.ConnectAsync(pi.HostName, "1");

            string sendData = "getmetrics";
            var writer = new DataWriter(socket.OutputStream);
            writer.WriteString(sendData);
            await writer.FlushAsync();
        }

        public async Task<bool> ConnectToHiveSense()
        {
            PeerFinder.AlternateIdentities["Bluetooth:PAIRED"] = "";
            var availableDevices = await PeerFinder.FindAllPeersAsync();
            if (availableDevices.Count == 0)
            {
                return false;
            }
            else
            {
                pi = (from ad in availableDevices
                      where ad.DisplayName.ToLower().StartsWith("hivesense")
                      select ad).FirstOrDefault();
                if (pi == default(PeerInformation))
                {
                    return false;
                }
            }

            return true;
        }

    }
}