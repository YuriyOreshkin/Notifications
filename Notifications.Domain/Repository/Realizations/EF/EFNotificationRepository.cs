using Notifications.Domain.Entities;
using Notifications.Domain.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Domain.Repository.Realizations.EF
{
    public class EFNotificationRepository : INotificationRepository
    {
        private NotificationsContext db = new NotificationsContext();
        private bool disposed = false;

        public IQueryable<Notification> Notifications 
        {
            get { return db.Notifications; }
        }


        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void CreateNotification(Notification notification)
        {
            db.Notifications.Add(notification);
            db.SaveChanges();
        }

        public Notification GetNotification(long id)
        {
            return db.Notifications.FirstOrDefault(n => n.ID == id);
        }

        public void Mark(Notification notification)
        {
            notification.Marked = !notification.Marked;
            db.Entry(notification).State = EntityState.Modified;
            db.SaveChanges();
        }

        public IQueryable<EmployeesNotification> GetEmployeeNotifications(long notificationId)
        {
            return  db.EmployeesNotifications.Where(n => n.NotificationID == notificationId);
        }

        public EmployeesNotification GetEmployeeNotification(long id)
        {
            return db.EmployeesNotifications.FirstOrDefault(e => e.ID == id);
        }


        public void Scanne(EmployeesNotification employeenotification)
        {
            employeenotification.Scanned = DateTime.Now;

            db.Entry(employeenotification).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void Mark(EmployeesNotification employeesnotification)
        {
            employeesnotification.Marked = !employeesnotification.Marked;

            db.Entry(employeesnotification).State = EntityState.Modified;
            db.SaveChanges();
        }

        public long CountEmployeeNotifications(long employeeId)
        {
            
            return db.EmployeesNotifications.Where(n => n.EmployeeID == employeeId && n.Scanned == null).Count();
        }

        public IQueryable<Connection> Connections
        {
            get { return db.Connections; }
        }

        public IQueryable<Priority> Priorities
        {
            get { return db.Priorities; }
        } 

        public void CreateConnection(Connection connection)
        {
            db.Connections.Add(connection);
            db.SaveChanges();
        }

        public void DeleteConnection(Connection connection)
        {
            db.Connections.Remove(connection);
            db.SaveChanges();
        }
    }
}
