using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Domain.Entities
{
    public class TreeViewEmployee
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool hasChildren { get; set;}
        public long? ParentId { get; set; }
    }
}
