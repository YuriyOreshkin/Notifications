using Microsoft.AspNet.SignalR;
using Ninject;
using Ninject.Modules;
using Notifications.Domain.Repository.Interfaces;
using Notifications.OuterService.XMPP.Interfaces;
using Notifications.WebUI.App_Start;
using Notifications.WebUI.Infrastructure;
using Notifications.WebUI.Models;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Notifications.WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Ninject 
            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());

            GlobalHost.DependencyResolver = new NinjectSignalRDependencyResolver(NinjectIoC.Initialize());

            //XMPP


                XMPPSettings xMPPSettings = XMPPConfig.ReadSettings();
                if (xMPPSettings.Enable)
                {
                    IXMPPClient sender = NinjectIoC.Initialize().Get<IXMPPClient>();
                    sender.Connect(xMPPSettings.Server, xMPPSettings.Port, xMPPSettings.User, xMPPSettings.Password, xMPPSettings.AutoReconnect);
                  
                }

        }

        protected void Application_PostAuthenticateRequest()
        {

            if (Request.IsAuthenticated)
            {

                IIdentity user = HttpContext.Current.User.Identity;

                IEmployeeRepository employeeRepository = NinjectIoC.Initialize().Get<IEmployeeRepository>();

                CustomPrincipal customPrincipal = new CustomPrincipal(user, employeeRepository);

                HttpContext.Current.User = customPrincipal;

              
        

            }
        }
    }
}
