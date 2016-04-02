using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hout.Models.Device
{
    public abstract class BaseExplorer
    {
        public delegate void DeviceFoundDelegate(DeviceFoundEventArgs args);
        public abstract Task StartScanning();
        public abstract Task StopScanning();
        public abstract event DeviceFoundDelegate OnDeviceFound;
    }
}
