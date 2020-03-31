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


            if (File.Exists(HttpContext.Current.Server.MapPath("~/App_Data/XMPPsettings.xml")))
            {
                XMPPSettings xMPPSettings = XMPPConfig.ReadSettings(HttpContext.Current.Server.MapPath("~/App_Data/XMPPsettings.xml"));
                if (xMPPSettings.Enable)
                {
                    IXMPPClient sender = NinjectIoC.Initialize().Get<IXMPPClient>();
                    sender.Connect(xMPPSettings.Server, xMPPSettings.Port, xMPPSettings.User, xMPPSettings.Password, xMPPSettings.AutoReconnect, HttpContext.Current.Server.MapPath("~/App_Data/XMPPlog.txt"));
                  
                }
            }
            else
            {
                XMPPSettings xMPPSettings = new XMPPSettings();
                xMPPSettings.Enable = false;
                xMPPSettings.Server = "127.0.0.1";
                xMPPSettings.Port = 8080;
                xMPPSettings.Domain = "domain.ru";
                xMPPSettings.AutoReconnect = 100;
                xMPPSettings.User = "user";
                xMPPSettings.Password = "password";
                xMPPSettings.MessageTemplate = "У Вас новое уведомление!";

                XMPPConfig.SaveSettings(xMPPSettings, HttpContext.Current.Server.MapPath("~/App_Data/XMPPsettings.xml"));

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
