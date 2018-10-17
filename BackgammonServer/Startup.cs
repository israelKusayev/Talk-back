using System;
using System.Threading.Tasks;
using BackgammonServer.DAL;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(BackgammonServer.Startup))]

namespace BackgammonServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();

            // visit https://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
