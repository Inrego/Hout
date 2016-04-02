using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Hout.Models.Device;

namespace Hout.Plugins.LimitlessLED.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var scanner = new LimitlessLEDExplorer();
            scanner.StartScanning().GetAwaiter().GetResult();
            var bulbs = new ConcurrentBag<LimitlessLEDWhite>();
            scanner.OnDeviceFound += eventArgs =>
            {
                Console.WriteLine($"{eventArgs.NewDevice.Name} - {eventArgs.NewDevice.Description}");
                for (sbyte i = 1; i <= 4; i++)
                {
                    var bulb = new LimitlessLEDWhite();
                    bulb.Address = (string) eventArgs.NewDevice.Properties["Address"];
                    bulb.Group = i;
                    bulbs.Add(bulb);
                }
            };
            string input;
            while ((input = Console.ReadLine()) != "exit")
            {
                foreach (var bulb in bulbs)
                {
                    bulb.ExecuteCommand(input, null).GetAwaiter().GetResult();
                }
            }
        }
    }
}
