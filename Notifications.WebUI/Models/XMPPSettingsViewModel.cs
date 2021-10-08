using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Notifications.WebUI.Models
{
    public class XMPPSettingsViewModel
    {
        public bool enable { get; set; }
        [Required]
        public string server { get; set; }
        [Required]
        public int port { get; set; }
        [Required]
        public string domain { get; set; }
        [Required]
        public float autoReconnect { get; set; }
        [Required]
        public string user { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string messageTemplate { get; set; }

        public bool isConnected { get; set; }
    }
}