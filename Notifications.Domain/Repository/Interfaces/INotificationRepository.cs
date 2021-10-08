using Notifications.Domain.Entities;
using System;
using System.Linq;

namespace Notifications.Domain.Repository.Interfaces
{
    public interface INotificationRepository : IDisposable
    {
        //Notification
        IQueryable<Notification> Notifications { get; }
        void CreateNotification(Notification notification);
        Notification GetNotification(long id);
        void Mark(Notification notification);

        //EmployeeNotification
        long CountEmployeeNotifications(long employeeId); 
        IQueryable<EmployeesNotification> GetEmployeeNotifications(long notificationId);
        EmployeesNotification GetEmployeeNotification(long id);
        void Scanne(EmployeesNotification employeenotification);
        void Mark(EmployeesNotification employeenotification);

        //Connections
        IQueryable<Connection> Connections { get; }
        void CreateConnection(Connection connection);
        void DeleteConnection(Connection connection);

        //Priorities
        IQueryable<Priority> Priorities { get; }

    }
}
