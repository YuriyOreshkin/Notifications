using System;
using System.Collections.Generic;
using System.Linq;

namespace Notifications.Domain.Entities
{
    public class Notification
    {
        public Notification()
        {
            this.EmployeesNotifications = new HashSet<EmployeesNotification>();
        }
        public long ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DateTime { get; set; }
        public long AuthorID { get; set; } 
        public short PriorityID { get; set; }
        public bool Marked { get; set; }

        public virtual Priority Priority { get; set; }
        public virtual ICollection<EmployeesNotification> EmployeesNotifications { get; set; }
    }
}
