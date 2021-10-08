using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.OuterService.XMPP.Interfaces
{
    public interface ILogger
    {
        void Info(string message);
        void Error(string message);

        IEnumerable<string> ReadLog(DateTime begin, DateTime end);
    }
}
