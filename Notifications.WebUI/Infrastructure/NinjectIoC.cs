using Ninject;
using Notifications.Domain.Repository.Interfaces;
using Notifications.Domain.Repository.Realizations.API;
using Notifications.Domain.Repository.Realizations.EF;
using Notifications.OuterService.XMPP.Interfaces;
using Notifications.OuterService.XMPP.Realizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notifications.WebUI.Infrastructure
{
    public static class NinjectIoC
    {
        public static IKernel Initialize()
        {
            IKernel kernel = new StandardKernel();
            AddBindings(kernel);
            return kernel;
        }

        private static IKernel AddBindings(IKernel ninjectKernel)
        {
            ninjectKernel.Bind<INotificationRepository>().To<EFNotificationRepository>();
            //ninjectKernel.Bind<IEmployeeRepository>().To<StaticEmployeeRepository>().InSingletonScope();
            ninjectKernel.Bind<IEmployeeRepository>().To<APIEmployeeRepository>().InSingletonScope().WithConstructorArgument("api_path",System.Configuration.ConfigurationManager.AppSettings["api"]);

            ninjectKernel.Bind<IXMPPClient>().To<JabberNet>().InSingletonScope();
            ninjectKernel.Bind<ILogger>().To<NLogger>().InSingletonScope().WithConstructorArgument("_logname", HttpContext.Current.Server.MapPath("~/App_Data/XMPPlog.txt"));
               
           // ninjectKernel.Bind<IEmployeeRepository>().To<APIEmployeeRepository>();
            return ninjectKernel;
        }
    }
}