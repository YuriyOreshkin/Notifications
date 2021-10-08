using System;
using System.Collections.Generic;
using System.Linq;
using Notifications.Domain.Entities;
using Notifications.Domain.Repository.Abstract;

namespace Notifications.Domain.Repository.Real
{
    public class StaticNotificationRepository
    {
        private List<Entities.Notification> notifications;
        public StaticNotificationRepository()
        {
            notifications = new List<Entities.Notification> {
                                                    new Entities.Notification { ID=1,
                                                                       AuthorID=1,
                                                                       Title = "Орешкин Ю.В.",
                                                                       Content ="Все будет хорошо!",
                                                                       DateTime =new DateTime(2019,04,24,4,4,5),
                                                                       EmployeesNotifications = new List<EmployeesNotification>{
                                                                            new EmployeesNotification { ID=1, DateTime=DateTime.Now, EmployeeID=1, NotificationID = 1 },
                                                                            new EmployeesNotification { ID=2, DateTime=DateTime.Now, EmployeeID=2, NotificationID = 1 }
                                                                       }

                                                    },
                                                    new Entities.Notification {  ID=2,
                                                                        AuthorID=2,
                                                                        Title = "Асистент",
                                                                        Content = " Добро <a href='http://10.7.0.80/IAPKomi/User/LogOn?ReturnUrl=%2fIAPKomi%2f'>ИАП</a> ",
                                                                        DateTime = new DateTime(2019,04,24,5,5,5),
                                                                        EmployeesNotifications = new List<EmployeesNotification>{
                                                                            new EmployeesNotification { ID=3, DateTime=DateTime.Now, EmployeeID=2, NotificationID = 2 },
                                                                            new EmployeesNotification { ID=4, DateTime=new DateTime(2019,04,24,5,5,5), EmployeeID=2, NotificationID = 2 }
                                                                       }
                                                    }
                                                  };
        }
        #region Notification
        public IQueryable<Entities.Notification> Notifications
        {
            get
            {
                return notifications.AsQueryable();
            }
        }


        public void CreateNotification(Entities.Notification notification)
        {
            notification.ID = notifications.Count() + 1;
            notification.DateTime = DateTime.Now;
            notifications.Add(notification);
        }

        public EmployeesNotification GetEmployeeNotification(long id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region EmployeeNotification
        public IQueryable<EmployeesNotification> GetEmployeeNotifications(long notificationId)
        {

            return notifications.FirstOrDefault(n=>n.ID == notificationId).EmployeesNotifications.AsQueryable();
            
        }

        public void Scanne(long id)
        {
            throw new NotImplementedException();
        }

        public void Scanne(EmployeesNotification employeenotification)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
