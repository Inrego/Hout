using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hout.Models.Device
{
    public class NewDeviceViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, object> Properties { get; set; }
    }
}
