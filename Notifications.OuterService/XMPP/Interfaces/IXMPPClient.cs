using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.OuterService.XMPP.Interfaces
{
    public interface IXMPPClient
    {
        string GetUserName(string name,string domain);
        bool IsAuthenticated();

        void Connect(string server, int port, string user, string password, float autoreconnect, string logname );
        void SendNotification(IEnumerable<string> sendto, string text);
    }
}
