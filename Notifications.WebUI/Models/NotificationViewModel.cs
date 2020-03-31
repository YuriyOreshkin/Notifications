using Notifications.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Notifications.WebUI.Models
{
    public class NotificationViewModel
    {

        public long id { get; set; }
        [Required]
        public string title { get; set; }
        [Required]
        [DisplayName("Контент")]
        [AllowHtml]
        public string content { get; set; }
        public DateTime datetime { get; set; }
        [DisplayName("Получатели")]
        [Required]
        public IEnumerable<long> sendto { get; set; }

        public EmployeeViewModel author { get; set; }
        

        public EmployeeNotificationViewModel employeenotification { get; set; }

        public PriorityViewModel priority { get; set; }

        public bool marked { get; set; }

        public Notification ToEntity(Notification notification)
        {
            notification.ID = this.id;
            notification.Title = this.title;
            notification.Content = WebUtility.HtmlDecode(this.content);
            notification.DateTime = DateTime.Now;
            notification.AuthorID = this.author.id;
            notification.PriorityID = this.priority.id;
            notification.EmployeesNotifications = this.sendto.Select(r => new EmployeesNotification
            {
                
                DateTime = DateTime.Now,
                EmployeeID = r,

            }).ToList<EmployeesNotification>();

            return notification;
        }
    }
}