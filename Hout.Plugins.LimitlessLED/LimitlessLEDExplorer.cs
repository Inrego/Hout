using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hout.Models.Device;

namespace Hout.Plugins.LimitlessLED
{
    public class LimitlessLEDExplorer : BaseExplorer
    {
        private UdpClient _udpClient;
        private bool _stop;
        private ConcurrentBag<NewDeviceViewModel> _foundDevices; 
        public override async Task StartScanning()
        {
            _stop = false;
            _foundDevices = new ConcurrentBag<NewDeviceViewModel>();
            using (var udpBroadcast = new UdpClient())
            {
                byte[] b = Encoding.UTF8.GetBytes("Link_Wi-Fi");
                udpBroadcast.Send(b, b.Length, "255.255.255.255", 48899);
            }
            _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 48899));
            Receive();
        }

        private void Receive()
        {
            _udpClient.BeginReceive(MyReceiveCallback, null);
        }
        private void MyReceiveCallback(IAsyncResult ar)
        {
            var ip = new IPEndPoint(IPAddress.Any, 48899);
            var result = _udpClient.EndReceive(ar, ref ip);
            if (!_stop)
            {
                Receive();
            }
            var resString = Encoding.UTF8.GetString(result);
            var regex = new Regex("^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5]),([A-F]|[0-9]){12},.*$");
            var isLimitlessLed = regex.IsMatch(resString);
            if (isLimitlessLed)
            {
                var device = new NewDeviceViewModel()
                {
                    Name = "LimitlessLED",
                    Description = "A LimitlessLED bulb Bridge located at " + ip,
                    Properties = new Dictionary<string, object>
                {
                    {"Address", ip.Address.ToString()}
                }
                };
                var existingDevice = _foundDevices.FirstOrDefault(d => (string) d.Properties["Address"] == ip.ToString());
                if (existingDevice == null)
                {
                    _foundDevices.Add(device);
                    OnDeviceFound?.Invoke(new DeviceFoundEventArgs(_foundDevices, device));
                }
            }
        }
        public override async Task StopScanning()
        {
            _stop = true;
            _udpClient.Client.Close();
        }

        public override event DeviceFoundDelegate OnDeviceFound;
    }
}
