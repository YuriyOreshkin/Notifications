using Kendo.Mvc.UI;
using Ninject;
using Notifications.Domain.Entities;
using Notifications.Domain.Repository.Interfaces;
using Notifications.WebUI.Extensions;
using Notifications.WebUI.Infrastructure;
using Notifications.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Notifications.WebUI.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
       

        public ActionResult Index()
        {
            ViewBag.Title = "Notifications";
            return View();
        }

        // GET: Notification
        public ActionResult Content(NotificationViewModel notification)
        {

            return PartialView(notification);
        }

        public ActionResult Recipients(long id)
        {
            return PartialView(id);
        }

        [AllowCrossSite] 
        public ActionResult ContentView(NotificationViewModel notification)
        {
            ViewBag.SendToTreeData = GetPossibleRecipients();
            ViewBag.Title = "Новое уведомление";
            return View(notification);
        }

        private IEnumerable<DropDownTreeItemModel> GetChildren(IEnumerable<TreeViewEmployee> recipients, long parentId, IEnumerable<Connection> connections)
        {
            
            return recipients.Where(r => r.ParentId == parentId)
                    .Select(o => new DropDownTreeItemModel
                    {
                        Id = o.ID.ToString(),
                        Text = o.Name,
                        Value = o.ID.ToString(),
                        Items = GetChildren(recipients, o.ID,connections).ToList(),
                        SpriteCssClass = connections.Any(e => e.EmployeeID == o.ID) ? "status-online" : ""

                    });
        }


        private IEnumerable<DropDownTreeItemModel> GetPossibleRecipients()
        {
            IEmployeeRepository employees = NinjectIoC.Initialize().Get<IEmployeeRepository>();
            INotificationRepository notification = NinjectIoC.Initialize().Get<INotificationRepository>();
            var possiblerecipients = employees.GetEmployeesWithDepartments("");
            var connections = notification.Connections.AsEnumerable();

            var rk = possiblerecipients.Where(r => r.ParentId == null).FirstOrDefault();
            List<DropDownTreeItemModel> treeview =possiblerecipients.Where(r=>r.ParentId  == null)
                .Select(o => new DropDownTreeItemModel
                    {
                        Id= o.ID.ToString(), 
                        Text = o.Name,
                        Value = o.ID.ToString(),
                        Items = GetChildren(possiblerecipients, o.ID,connections).ToList(),
                        SpriteCssClass = connections.Any(e=>e.EmployeeID == o.ID) ? "status-online" : ""

                    }).ToList();

            return treeview;
        }


    

        public ActionResult RecipientsView(long id)
        {
            ViewBag.Title = "Получатели";
            return View(id);
        }
    }
}