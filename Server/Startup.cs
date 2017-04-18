using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Server.Startup))]

namespace Server
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
            app.MapSignalR();
        }
    }
}