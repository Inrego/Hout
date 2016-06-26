using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hout.Models;
using Hout.Models.Db;
using Hout.Models.Device;
using Hout.Plugins.LimitlessLED;
using Hout.Service.ExplorerHandling;
using Microsoft.AspNet.SignalR;

namespace Hout.Service.Hubs
{
    public class DevicesHub : Hub
    {
        private static readonly Lazy<IHubContext> Instance = new Lazy<IHubContext>(
        () => GlobalHost.ConnectionManager.GetHubContext<DevicesHub>());
        public async Task<IEnumerable<BaseDevice>> GetDevices()
        {
            using (var db = new Client())
            {
                return await db.GetDevices();
            }
        }

        public async Task<IEnumerable<object>> GetExplorers()
        {
            var explorerType = typeof (BaseExplorer);
            var explorerTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => explorerType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
            var explorers = explorerTypes.Select(t => (BaseExplorer) Activator.CreateInstance(t));
            return explorers;
        }
        public async Task TestDevice(NewDeviceViewModelSimple model)
        {
            var type = Type.GetType(model.Type);
            var explorer = (BaseExplorer) Activator.CreateInstance(type);
            await explorer.TestDevice(model);
        }

        public async Task AddDevice(NewDeviceViewModelSimple viewModel)
        {
            var explorerType = Type.GetType(viewModel.Type);
            var explorer = (BaseExplorer) Activator.CreateInstance(explorerType);
            var device = await explorer.GetDevice(viewModel);
            BaseDevice.PopulateDefaultValues(device);
            using (var db = new Client())
            {
                await db.AddOrUpdate(device);
            }
            Clients.All.deviceAdded(device);
        }

        public async Task StartScan(string type)
        {
            await Groups.Add(Context.ConnectionId, type);
            await ExplorerHandler.StartExplorer(type);
        }

        public static void DeviceFound(NewDeviceViewModel args)
        {
            Instance.Value.Clients.Group(args.Type).deviceFound(args);
        }
    }
}
