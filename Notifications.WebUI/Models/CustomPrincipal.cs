using Newtonsoft.Json;
using Notifications.Domain.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Notifications.WebUI.Models
{
    public class CustomPrincipal : IPrincipal
    {
        IIdentity identity;
        private long _id;
        private string _login;

        public string Login{ get { return this._login; }}
        public long UserID { get { return this._id; } }

        public CustomPrincipal(IIdentity identity, IEmployeeRepository employeeRepository)
        {
            this.identity = identity;
            _login = identity.Name.Split('\\')[1];
            var user = employeeRepository.GetEmployee(_login);
            if (user != null)
            {
                _id = user.ID;
            }
            else
            {
                _id = 0;
            }
        }

        public IIdentity Identity
        {
            get { return this.identity; }
        }

        public bool IsInRole(string role)
        {
            return HttpContext.Current.User.IsInRole(role);
        }

    }
}