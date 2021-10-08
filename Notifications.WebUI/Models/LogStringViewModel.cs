using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notifications.WebUI.Models
{
    public class LogStringViewModel
    {
        public DateTime date { get; set; }
        public TimeSpan time { get; set; }
        public string type { get; set; }
        public string content { get; set; }
    }
}