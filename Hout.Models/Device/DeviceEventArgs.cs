using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hout.Models.Device
{
    public class DeviceEventArgs
    {
        public DeviceEventArgs(string eventName, Dictionary<string, object> parameters)
        {
            EventName = eventName;
            Parameters = parameters;
        }
        public string EventName { get; private set; }
        public Dictionary<string, object> Parameters { get; private set; } 
    }
}
