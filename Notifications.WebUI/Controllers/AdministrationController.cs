using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Notifications.OuterService.XMPP.Interfaces;
using Notifications.WebUI.App_Start;
using Notifications.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Notifications.WebUI.Controllers
{
    public class AdministrationController : Controller
    {
        private IXMPPClient sender;
        public AdministrationController(IXMPPClient _sender) {

            sender = _sender;
        }
        // GET: Settings
        public ActionResult Settings()
        {
            XMPPSettings xMPPSettings = XMPPConfig.ReadSettings();

            var result = new XMPPSettingsViewModel
            {
                server = xMPPSettings.Server,
                domain = xMPPSettings.Domain,
                port = xMPPSettings.Port,
                autoReconnect = xMPPSettings.AutoReconnect,
                enable = xMPPSettings.Enable,
                user = xMPPSettings.User,
                password = xMPPSettings.Password,
                messageTemplate = xMPPSettings.MessageTemplate,
                isConnected = sender.IsConnected()
            };

            return PartialView(result);
        }

       
        public ActionResult Log()
        {

            return PartialView();
        }



        public ActionResult ReadLog([DataSourceRequest]DataSourceRequest request, DateTime datebegin, DateTime dateend)
        {
            var log = sender.ReadLog(datebegin, dateend).Select(line =>
            new LogStringViewModel
            {
                date = DateTime.Parse(line.Split('|')[0]).Date,
                time = DateTime.Parse(line.Split('|')[0]).TimeOfDay,
                type = line.Split('|')[1],
                content = line.Split('|')[3]
            }).OrderByDescending(d=>d.date).ThenByDescending(t=>t.time);

            JsonResult result = Json(log.ToDataSourceResult(request));
            result.MaxJsonLength = 8675309;


            return  result;
        }



        public JsonResult SaveSettings(XMPPSettingsViewModel settings)
        {
            XMPPSettings xMPPSettings = new XMPPSettings();

            xMPPSettings.Server = settings.server;
            xMPPSettings.Domain = settings.domain;
            xMPPSettings.Port = settings.port;
            xMPPSettings.AutoReconnect = settings.autoReconnect;
            xMPPSettings.Enable = settings.enable;
            xMPPSettings.User = settings.user;
            xMPPSettings.Password = settings.password;
            xMPPSettings.MessageTemplate = settings.messageTemplate;


            //Save
            try
            {

                XMPPConfig.SaveSettings(xMPPSettings);
            }
            catch (Exception exception)
            {
                return Json(new { message = "errors", errors = "Ошибка: " + exception.Message }, JsonRequestBehavior.AllowGet);
            }

            //Reconnect
            try
            {
                sender.Close();

                if (settings.enable)
                {
                    sender.Connect(settings.server, settings.port, settings.user, settings.password, settings.autoReconnect);
                    Thread.Sleep(1000);
                }
                settings.isConnected = sender.IsConnected();

            }
            catch (Exception exception)
            {
                return Json(new { message = "errors", errors = "Ошибка: " + exception.Message }, JsonRequestBehavior.AllowGet);
            }

           

            return Json(new { message = "OK", result = settings }, JsonRequestBehavior.AllowGet);
        }
    }
}