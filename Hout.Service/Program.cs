using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Hout.Models;
using Hout.Models.Db;
using Hout.Models.Device;
using Hout.Plugins.LimitlessLED;


namespace Hout.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(params string[] args)
        {
            if (args.Contains("launch"))
                Process.Start("http://localhost:5252/");
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
