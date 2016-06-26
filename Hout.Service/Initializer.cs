using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hout.Models.Db;
using Microsoft.Owin.Hosting;

namespace Hout.Service
{
    public class Initializer
    {
        public async Task Initialize()
        {
            using (var client = new Client())
            {
                await client.Initialize();
                await client.EnsureTables();
            }
            WebApp.Start("http://*:5252");
        }
    }
}
