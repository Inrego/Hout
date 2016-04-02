using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hout.Models.Device
{
    public class DeviceFoundEventArgs
    {
        public DeviceFoundEventArgs(ConcurrentBag<NewDeviceViewModel> foundDevices, NewDeviceViewModel newDevice)
        {
            FoundDevices = foundDevices;
            NewDevice = newDevice;
        }
        public ConcurrentBag<NewDeviceViewModel> FoundDevices { get; private set; }
        public NewDeviceViewModel NewDevice { get; private set; }
    }
}
