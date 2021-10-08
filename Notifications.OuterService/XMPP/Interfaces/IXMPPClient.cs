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
        bool IsConnected();
        void Connect(string server, int port, string user, string password, float autoreconnect);
        void Close();
        void SendNotification(IEnumerable<string> sendto, string text);

        IEnumerable<string> ReadLog(DateTime begin, DateTime end);
    }
}
