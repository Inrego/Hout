using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using Hout.Models;
using Hout.Models.Device;
using Hout.Plugins.LimitlessLED;
using Raven.Client.Document;


namespace Hout.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(params string[] args)
        {
            using (var store = new DocumentStore
            {
                Url = "http://live-test.ravendb.net/",
                DefaultDatabase = "Hout",
                Conventions =
                {
                    FindTypeTagName = type =>
                    {
                        if (typeof(BaseDevice).IsAssignableFrom(type))
                            return "BaseDevice";
                        return DocumentConvention.DefaultTypeTagName(type);
                    }
                }
            })
            {
                store.Initialize();
                using (var session = store.OpenSession())
                {
                    var device = new LimitlessLEDWhite();
                    device.Address = "192.168.0.47";
                    device.Group = 1;
                    device.Id = device.GetId();

                    session.Store(device);
                    session.SaveChanges();

                    var loadedDevice = session.Load<BaseDevice>();
                    Debugger.Break();
                }
            }
                return;


            if (args.Contains("launch"))
                System.Diagnostics.Process.Start("http://localhost:5252/");
            if (!IsConsole)
            {
                var servicesToRun = new ServiceBase[]
                {
                    new Service1()
                };
                ServiceBase.Run(servicesToRun);
            }
            else
            {
                var initializer = new Initializer();
                initializer.Initialize();
                Console.ReadLine();
            }
        }

        private static bool? _isConsole;
        public static bool IsConsole
        {
            get
            {
                if (!_isConsole.HasValue)
                {
                    try
                    {
                        var windowHeight = Console.WindowHeight;
                        _isConsole = true;
                    }
                    catch
                    {
                        _isConsole = false;
                    }
                }
                return _isConsole.Value;
            }
        }
    }
}
