using Notifications.Domain.Repository.Interfaces;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Notifications.WebUI.Models;
using Notifications.WebUI.Hubs;
using System.Collections.Generic;
using System;
using System.Data.Entity;
using Notifications.Domain.Entities;
using Notifications.OuterService.XMPP.Interfaces;
using Notifications.WebUI.Infrastructure;
using Ninject;
using Notifications.WebUI.App_Start;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Threading;

namespace Notifications.WebUI.Controllers
{
    public class NotificationDirectoryController : Controller
    {
       
        private INotificationRepository notificationRepository;
        private IEmployeeRepository employeeRepository;


        public NotificationDirectoryController(INotificationRepository _notificationRepository, IEmployeeRepository _employeeRepository)
        {
            notificationRepository = _notificationRepository;
            employeeRepository = _employeeRepository;
            
        }


        public ActionResult ReadNotifications([DataSourceRequest] DataSourceRequest request,int type, bool scanned,bool marked,long? recipient)
        {
            //TODO
            var author =((CustomPrincipal)User).UserID;//employeeRepository.GetEmployee(User.Identity.Name.Split('\\')[1]).ID;

            var notifications = notificationRepository.Notifications;


            if (type == 0)
            {
                //IN
                //notifications = notifications.Where(n => n.EmployeesNotifications.Select(s => s.EmployeeID).Contains(author));
                notifications = notifications.Where(n => n.EmployeesNotifications.Where(e => e.EmployeeID == author && (scanned ? true : e.Scanned == null ) && (marked ? e.Marked : true)).Count() > 0);
            }
            else
            {
                //OUT
                notifications = notifications.Where(n => n.AuthorID == author && (marked ? n.Marked : true));
            }

            //filter Recipient
            if (recipient.HasValue)
            {
                notifications = notifications.Where(n => n.EmployeesNotifications.Select(s => s.EmployeeID).Contains((long)recipient));
            }


            var result = notifications.Include(p => p.Priority).Include(n => n.EmployeesNotifications).ToDataSourceResult(request,n => new NotificationViewModel
             {
                 id = n.ID,
                 author =ConvertToViewModel(employeeRepository.GetEmployee(n.AuthorID)),
                 title = n.Title,
                 content = n.Content,
                 datetime = n.DateTime,
                 employeenotification = type == 0 ? ConvertToViewModel(n.EmployeesNotifications.Where(e => e.EmployeeID == author).OrderByDescending(o => o.DateTime).FirstOrDefault()) : ConvertToViewModel(n.EmployeesNotifications.FirstOrDefault()),    //null,
                 sendto = n.EmployeesNotifications.ToList().Select(r => r.EmployeeID),
                 priority = ConvertToViewModel(n.Priority),
                 marked = type == 0 ? n.EmployeesNotifications.Where(e => e.EmployeeID == author).OrderByDescending(o => o.DateTime).FirstOrDefault().Marked : n.Marked
             });

            return Json(result);

        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult CreateNotification(NotificationViewModel _notification)
        {
            //TODO
            var author = _notification.author == null ? ((CustomPrincipal)User).UserID : _notification.author.id;// employeeRepository.GetEmployee( User.Identity.Name.Split('\\')[1]).ID;

            var entity = new Notifications.Domain.Entities.Notification();

            //author
            _notification.author = ConvertToViewModel(employeeRepository.GetEmployee(author));

            _notification.title = string.IsNullOrEmpty(_notification.title) ? _notification.author.fullname : _notification.title;

            //recipients
            IEnumerable<long> recipients = Enumerable.Empty<long>();
            _notification.sendto.Where(s => s < 0).ToList().ForEach(o =>
            {
                GetAllChilds(o, ref recipients);
            });

            _notification.sendto = _notification.sendto.Where(s => s > 0).Concat(recipients.Where(r => r > 0)).Distinct();
            if (_notification.sendto.Count() > 0)
            {

                _notification.ToEntity(entity);
                try
                {
                    notificationRepository.CreateNotification(entity);

                    _notification.id = entity.ID;
                    _notification.content = entity.Content;
                    _notification.datetime = entity.DateTime;
                    _notification.priority = ConvertToViewModel(notificationRepository.Priorities.FirstOrDefault(p => p.ID == entity.PriorityID));

                    _notification.employeenotification = ConvertToViewModel(entity.EmployeesNotifications.FirstOrDefault());

                    //New Notification
                    displayNewNotification(_notification);

                    //Create success Notification
                    displayAddNotification(_notification);

                    //Out Send
                    try
                    {

                        XMPPSettings xMPPSettings = XMPPConfig.ReadSettings();
                        if (xMPPSettings.Enable)
                        {
                            SendNotificationOut(_notification, xMPPSettings);
                        }
                    }
                    catch { } /*(Exception exc)
                    {
                        return Json(new { message = "errors", errors = "Ошибка: " + exc.Message }, JsonRequestBehavior.AllowGet);
                    }*/

                    return Json(new { message = "OK", result = _notification }, JsonRequestBehavior.AllowGet);
                  

                }
                catch (Exception exc)
                {

                    return Json(new { message = "errors", errors = "Ошибка: " + exc.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else {

                return Json(new { message = "errors", errors = "Ошибка: Не найден ни один получатель!" }, JsonRequestBehavior.AllowGet);

            }
         }



        //Send XMPP message
        [AllowAnonymous]
        public JsonResult SendNotificationXMPP(NotificationViewModel _notification)
        {
            //recipients
            IEnumerable<long> recipients = Enumerable.Empty<long>();
            _notification.sendto.Where(s => s < 0).ToList().ForEach(o =>
            {
                GetAllChilds(o, ref recipients);
            });

            _notification.sendto = _notification.sendto.Where(s => s > 0).Concat(recipients.Where(r => r > 0)).Distinct();


            if (_notification.sendto.Count() > 0)
            {

                try
                {

                    XMPPSettings xMPPSettings = XMPPConfig.ReadSettings();
                    if (xMPPSettings.Enable)
                    {
                        xMPPSettings.MessageTemplate = "{notification.content}";
                        SendNotificationOut(_notification, xMPPSettings);
                    }
                    else
                    {
                        return Json(new { message = "errors", errors = "Ошибка: Сервис рассылки отключен!"}, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new { message = "OK", result = _notification }, JsonRequestBehavior.AllowGet);
                }
                catch(Exception exc)
                {

                    return Json(new { message = "errors", errors = "Ошибка: " + exc.Message }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {

                return Json(new { message = "errors", errors = "Ошибка: Не найден ни один получатель!" }, JsonRequestBehavior.AllowGet);

            }

        }

        //Send XMPP message
        private void SendNotificationOut(NotificationViewModel notification, XMPPSettings xMPPSettings)
        {
            IXMPPClient sender = NinjectIoC.Initialize().Get<IXMPPClient>();
            string[] xmppRecipients = notification.sendto.Select(s => sender.GetUserName(employeeRepository.GetEmployee(s).Login, xMPPSettings.Domain)).ToArray<string>();

            if (!sender.IsConnected())
            {
                sender.Connect(xMPPSettings.Server, xMPPSettings.Port, xMPPSettings.User, xMPPSettings.Password, xMPPSettings.AutoReconnect);
                Thread.Sleep(1000);
            }

            Task.Run(() =>
             {

                  sender.SendNotification(xmppRecipients, FullTemplate(notification, xMPPSettings.MessageTemplate));
                
            });
        }

        //
        private string FullTemplate(NotificationViewModel notification, string text)
        {
           
            Dictionary<string,string> rules = new Dictionary<string, string>();
           //var objj = typeof(PriorityViewModel).GetProperty("priority").GetValue(notification);
            GetLookups(typeof(NotificationViewModel).GetProperties(), notification, "notification", ref rules);


            string result = text;
            foreach(KeyValuePair<string,string>  rule in rules){
                Regex regex = new Regex("{" + rule.Key + "}", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                result = regex.Replace(result, rule.Value);
            }

            return result;
        }

        private void GetLookups(PropertyInfo[] properties, object obj,string anchor, ref Dictionary<string, string> rules)
        {
            if (obj != null)
            {
                
                foreach (PropertyInfo property in properties)
                {
                    
                    if (property.PropertyType.Namespace.Contains("System"))
                    {
                        if (property.GetValue(obj) != null)
                        {
                            rules.Add(anchor + "." + property.Name, property.GetValue(obj).ToString());
                        }
                        else
                        {
                            rules.Add(anchor + "." + property.Name, "null");
                        }

                    }
                    else
                    {
                        
                        GetLookups(property.PropertyType.GetProperties(), property.GetValue(obj),anchor + "." + property.Name, ref rules);
                    }
                }
            }
        }

        //Read for searcher
        public JsonResult ReadEmployees(long? notificationId, string text)
        {
            var filter = "";

            if (!string.IsNullOrEmpty(text))
            {
                filter = String.Format("FIO~contains~'{0}'", text);
            }

            var recipiets = notificationId.HasValue ? notificationRepository.GetEmployeeNotifications((long)notificationId) : null ;
            var employees = employeeRepository.GetEmployees(filter).Where(e=> recipiets != null ? recipiets.Select(r=>r.EmployeeID).Contains(e.ID) : true).Select(e => new EmployeeViewModel
            {
                id = e.ID,
                fullname = e.FullName

            });

            return Json(employees, JsonRequestBehavior.AllowGet);

        }
        

        private void GetAllChilds(long id, ref IEnumerable<long> result)
        {
            var childs = employeeRepository.GetEmployeesWithDepartments(String.Format("parentID~eq~'{0}'", id.ToString()));
            result = result.Concat(childs.Select(s => s.ID));

           foreach (var child in childs.Where(c => c.ID < 0))
           {
                    GetAllChilds(child.ID, ref result);
           }
            
        }

        //SignalR
        private void displayNewNotification(NotificationViewModel notification)
        {
            
            var context =Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<NotificationsHub>();

            var recipientConns = notificationRepository.Connections.Where(c => notification.sendto.Contains(c.EmployeeID)).Select(s => s.ConnectionID).ToList();
            
            context.Clients.Clients(recipientConns).displayNewNotification(notification);
        }

        private void displayAddNotification(NotificationViewModel notification)
        {
            var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<NotificationsHub>();
            var authorConns = notificationRepository.Connections.Where(c =>c.EmployeeID == notification.author.id).Select(s => s.ConnectionID).ToList();

            context.Clients.Clients(authorConns).displayAddNotification(notification);
        }



        public JsonResult CountNew()
        {
            
            var author = ((CustomPrincipal)User).UserID; //employeeRepository.GetEmployee(User.Identity.Name.Split('\\')[1]).ID;
            var result = notificationRepository.CountEmployeeNotifications(author);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadRecipients([DataSourceRequest] DataSourceRequest request, long notificationId)
        {

            var result = notificationRepository.GetEmployeeNotifications(notificationId).ToDataSourceResult(request, e=> new EmployeeNotificationViewModel
            {
                id = e.ID,
                recipient = ConvertToViewModel(employeeRepository.GetEmployee(e.EmployeeID)),
                scanned = e.Scanned

            });


            return Json(result);
            
        }

        public JsonResult ReadPriorities()
        {
            var priorities = notificationRepository.Priorities.OrderByDescending(o=>o.ID).ToList().Select(p => ConvertToViewModel(p));

            return Json(priorities, JsonRequestBehavior.AllowGet);
        }

        //Employee Entity to ViewModel
        private EmployeeViewModel ConvertToViewModel(Employee employee)
        {
            return new EmployeeViewModel
            {
                id = employee.ID,
                fullname = employee.FullName,
                online =notificationRepository.Connections.Any(o=>o.EmployeeID == employee.ID)
            };
        }

        //Employee Entity to ViewModel
        private EmployeeNotificationViewModel ConvertToViewModel(EmployeesNotification employeesnotification)
        {
            if (employeesnotification != null)
            {
                return new EmployeeNotificationViewModel
                {
                    id = employeesnotification.ID,
                    recipient = ConvertToViewModel(employeeRepository.GetEmployee(employeesnotification.EmployeeID)),
                    scanned = employeesnotification.Scanned

                };
            }
            else
            {
                return null;
            }
        }

        private PriorityViewModel ConvertToViewModel(Priority priority)
        {
            return new PriorityViewModel
            {
                id= priority.ID,
                color= priority.Color,
                description=priority.Description
            };
        }

        //Scanne notification
        public ActionResult Scanne(long id)
        {
            var entity = notificationRepository.GetEmployeeNotification(id);
            if(entity == null)
                return Json(new { message = "errors", result = "Уведомление не найдено!" }, JsonRequestBehavior.AllowGet);

            try
            {
                notificationRepository.Scanne(entity);

                var view = ConvertToViewModel(entity);

                return Json(new { message = "OK", result = view }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception exc)
            {

                return Json(new { message = "errors", result = exc.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        //Mark
        public ActionResult Mark(int type,long id)
        {
                try
                {
                    if (type == 0)
                    {
                        notificationRepository.Mark(notificationRepository.GetEmployeeNotification(id));
                    }
                    else
                    {
                        notificationRepository.Mark(notificationRepository.GetNotification(id));
                    }

                    return Json(new { message = "OK" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {

                    return Json(new { message = "errors", result = exc.Message }, JsonRequestBehavior.AllowGet);
                }
           
        }

        protected override void Dispose(bool disposing)
        {

            base.Dispose(disposing);
        }

    }

}
