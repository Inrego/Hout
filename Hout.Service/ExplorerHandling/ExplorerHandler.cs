using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hout.Models.Device;
using Hout.Service.Hubs;

namespace Hout.Service.ExplorerHandling
{
    public static class ExplorerHandler
    {
        private static readonly ConcurrentDictionary<Type, ExplorerWrapper> ExplorerWrappers = new ConcurrentDictionary<Type, ExplorerWrapper>();
        public static async Task StartExplorer(string typeName)
        {
            var type = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .First(t => t.FullName == typeName);
            ExplorerWrapper wrapper;
            var running = ExplorerWrappers.TryGetValue(type, out wrapper);
            if (running)
            {
                await wrapper.RestartScan();
            }
            else
            {
                wrapper = new ExplorerWrapper((BaseExplorer) Activator.CreateInstance(type));
                wrapper.OnScanExpired += Wrapper_OnScanExpired;
                wrapper.OnDeviceFound += Explorer_OnDeviceFound;
                ExplorerWrappers.TryAdd(type, wrapper);
                Task.Run(wrapper.Scan);
            }
        }
        private static void Explorer_OnDeviceFound(DeviceFoundEventArgs args)
        {
            DevicesHub.DeviceFound(args.NewDevice);
        }

        private static void Wrapper_OnScanExpired(object sender, EventArgs e)
        {
            var removedItem = (ExplorerWrapper) sender;
            ExplorerWrappers.TryRemove(removedItem.Explorer.GetType(), out removedItem);
        }
    }
}
