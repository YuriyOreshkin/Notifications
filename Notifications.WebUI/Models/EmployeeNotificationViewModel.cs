using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notifications.WebUI.Models
{
    public class EmployeeNotificationViewModel
    {
        public long id { get; set; }
        public EmployeeViewModel recipient { get; set; }
        public DateTime? scanned { get; set; }
    }
}