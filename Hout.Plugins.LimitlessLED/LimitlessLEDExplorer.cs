using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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
        public override string Name => "LimitlessLED";

        public override async Task StartScanning()
        {
            _stop = false;
            _foundDevices = new ConcurrentBag<NewDeviceViewModel>();
            _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 48899));
            Receive();
            using (var udpBroadcast = new UdpClient())
            {
                byte[] b = Encoding.UTF8.GetBytes("Link_Wi-Fi");
                udpBroadcast.Send(b, b.Length, "255.255.255.255", 48899);
            }
        }

        private void Receive()
        {
            _udpClient.BeginReceive(MyReceiveCallback, null);
        }
        private void MyReceiveCallback(IAsyncResult ar)
        {
            if (_stop)
                return;
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
                var device = new LimitlessLEDWhite();
                for (var i = 1; i < 5; i++)
                {
                    var viewModel = new NewDeviceViewModel($"LimitlessLED Group {i}", $"A LimitlessLED bulb Bridge located at {ip}",
                    new PropertyCollection
                    {
                        {"Address", ip.Address.ToString()},
                        {"Group", i }
                    }, device.PropertySpecifications, GetType());
                    var existingDevice = _foundDevices.FirstOrDefault(d => (string)d.Properties["Address"] == ip.ToString());
                    if (existingDevice == null)
                    {
                        _foundDevices.Add(viewModel);
                        OnDeviceFound?.Invoke(new DeviceFoundEventArgs(_foundDevices, viewModel));
                    }
                }
            }
        }
        public override async Task StopScanning()
        {
            _stop = true;
            _udpClient.Client.Close();
        }

        public override async Task TestDevice(NewDeviceViewModelSimple model)
        {
            var device = new LimitlessLEDWhite();
            foreach (var property in model.Properties)
            {
                device.Properties[property.Key] = property.Value;
            }
            device.Group = Convert.ToSByte(device.Properties["Group"]);
            await device.ExecuteCommand("Turn Off");
            await Task.Delay(1000);
            await device.ExecuteCommand("Turn On");
        }

        public override async Task<BaseDevice> GetDevice(NewDeviceViewModelSimple viewModel)
        {
            var device = new LimitlessLEDWhite();
            device.Properties = viewModel.Properties;
            device.Name = viewModel.Name;
            return device;
        }

        public override event DeviceFoundDelegate OnDeviceFound;
    }
}
