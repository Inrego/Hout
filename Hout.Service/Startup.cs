using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beginor.Owin.StaticFile;
using Microsoft.Owin.Cors;
using Owin;

namespace Hout.Service
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();

            app.UseStaticFile(new StaticFileMiddlewareOptions
            {
                RootDirectory = @".\web",
                DefaultFile = "index.html",
                EnableETag = true,
                MimeTypeProvider = new MimeTypeProvider()
            });
        }
    }
}
