using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Notifications.WebUI.Startup))]
namespace Notifications.WebUI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            app.MapSignalR();
           
            //GlobalHost.HubPipeline.RequireAuthentication();
        }
    }
}