using System;

namespace Notifications.Domain.Entities
{
    public class EmployeesNotification
    {
        public long ID { get; set; }
        public DateTime DateTime { get; set; }
        public long EmployeeID { get; set; }
        public long NotificationID { get; set; }
        public Nullable<DateTime> Scanned { get; set; }
        public bool Marked { get; set; }

        public virtual Notification Notification { get; set; }
    }
}
