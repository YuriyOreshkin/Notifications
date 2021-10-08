using System.Collections.Generic;

namespace Notifications.Domain.Entities
{
    public class Priority
    {
        public Priority()
        {
            this.Notification = new HashSet<Notification>();
        }
        public short ID { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Notification> Notification { get; set; }
    }
}
