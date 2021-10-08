using Microsoft.AspNet.SignalR;
using Notifications.Domain.Entities;
using Notifications.Domain.Repository.Interfaces;
using Notifications.WebUI.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Notifications.WebUI.Hubs
{
   [Authorize]
    public class NotificationsHub : Hub
    {
        private INotificationRepository notificationRepository;
       //private IEmployeeRepository employeeRepository;
        public NotificationsHub(INotificationRepository _notificationRepository)//,IEmployeeRepository _employeeRepository)
        {
            notificationRepository = _notificationRepository;
           // employeeRepository = _employeeRepository;
        }

      
        public override Task OnConnected()
        {
            var id = Context.ConnectionId;
            var employeeID = ((CustomPrincipal)Context.User).UserID;//employeeRepository.GetEmployee(Context.User.Identity.Name.Split('\\')[1]).ID;

            if (!notificationRepository.Connections.Any(x => x.ConnectionID == id))
            {
                notificationRepository.CreateConnection(new Connection { ConnectionID = id, EmployeeID = employeeID });
                
                //Clients.Caller.onConnected(id, userName, Users);
                
                //Clients.AllExcept(id).onNewUserConnected(id, userName);
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var item = notificationRepository.Connections.FirstOrDefault(x => x.ConnectionID == Context.ConnectionId);
            if (item != null)
            {
                notificationRepository.DeleteConnection(item);
                //Clients.All.onUserDisconnected(id, item.Name);
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}