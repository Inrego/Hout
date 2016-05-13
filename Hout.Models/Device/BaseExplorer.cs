﻿using System;
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
        public abstract Task TestDevice(NewDeviceViewModel model);
        public abstract event DeviceFoundDelegate OnDeviceFound;
        public abstract string Name { get; }
        public string Type => GetType().FullName;
    }
}
