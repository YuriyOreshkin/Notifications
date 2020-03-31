using Notifications.Domain.Entities;
using System.Data.Entity;

namespace Notifications.Domain.Repository.Realizations.EF
{
    public class NotificationsContext : DbContext
    {
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<EmployeesNotification> EmployeesNotifications { get; set; } 
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Priority> Priorities { get; set; }
    }
}

