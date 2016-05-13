using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Hout.Models.Device;

namespace Hout.Service.ExplorerHandling
{
    public class ExplorerWrapper
    {
        public BaseExplorer Explorer { get; private set; }
        private CancellationTokenSource _restartToken;
        public event EventHandler OnScanExpired;
        public event BaseExplorer.DeviceFoundDelegate OnDeviceFound;
        public ExplorerWrapper(BaseExplorer explorer)
        {
            Explorer = explorer;
        }
        public DeviceFoundEventArgs LastResponse { get; private set; }
        public async Task Scan()
        {
            if (_restartToken != null)
                return;
            Explorer.OnDeviceFound += _explorer_OnDeviceFound;
            await Explorer.StartScanning();
            await WaitTimer();
            await Explorer.StopScanning();
        }

        public async Task RestartScan()
        {
            _restartToken.Cancel();
            await Explorer.StopScanning();
            Explorer.OnDeviceFound -= _explorer_OnDeviceFound;
            Explorer = (BaseExplorer) Activator.CreateInstance(Explorer.GetType());
            Explorer.OnDeviceFound += _explorer_OnDeviceFound;
            await Explorer.StartScanning();
        }
        private async Task WaitTimer()
        {
            Task waitTask;
            do
            {
                _restartToken = new CancellationTokenSource();
                waitTask = Task.Delay(10000, _restartToken.Token);
                try
                {
                    await waitTask;
                }
                catch (TaskCanceledException)
                {
                    
                }
            } while (waitTask.IsCanceled);
            OnScanExpired?.Invoke(this, EventArgs.Empty);

        }
        private void _explorer_OnDeviceFound(DeviceFoundEventArgs args)
        {
            LastResponse = args;
            OnDeviceFound?.Invoke(args);
        }
    }
}
