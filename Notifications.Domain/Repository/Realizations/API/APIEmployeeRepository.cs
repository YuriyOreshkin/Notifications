using Newtonsoft.Json;
using Notifications.Domain.Entities;
using Notifications.Domain.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Notifications.Domain.Repository.Realizations.API
{
    public class APIEmployeeRepository : IEmployeeRepository
    {
        private string api_path;
        public APIEmployeeRepository(string api_path)
        {
            this.api_path = api_path;
        }

        public IQueryable<Employee> GetEmployees(string filter)
        {
          
                using (var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
                {
                    UriBuilder builder = new UriBuilder(api_path + "USERS/Users_Read");
                    var parameters = new Dictionary<string, string>
                            {
                               { "sort", "FIO-asc" },
                               { "page", "1" },
                               { "pageSize","20" },
                               { "group",""},
                               { "filter", filter}
                            };
                    var content = new FormUrlEncodedContent(parameters);
                    var response = client.PostAsync(builder.Uri,content).Result;
                    var responseString = response.Content.ReadAsStringAsync().Result;
                    dynamic obj = JsonConvert.DeserializeObject(responseString);
                    List<Employee> result = new List<Employee>();
                    foreach (var employee in obj.Data)
                    {
                        result.Add(new Employee { ID = employee.ID, FullName = employee.FIO, Login = employee.Login });
                    }

                        return result.AsQueryable();

                }
        }



        public IQueryable<Employee> GetEmployeesByDepartment(string code)
        {
            using (var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
            {
                UriBuilder builder = new UriBuilder(api_path + "api/getUsersByOrg");
                builder.Query = "ident="+code;
                var response = client.GetAsync(builder.Uri).Result;
                response.EnsureSuccessStatusCode();
                var responseString = response.Content.ReadAsStringAsync().Result;
                dynamic obj = JsonConvert.DeserializeObject(responseString);

                List<Employee> result = new List<Employee>();
                foreach (var employee in obj)
                {
                    result.Add(new Employee { ID = employee.ID,  FullName =String.Format("{0} {1} {2}",employee.LastName,employee.FirstName, employee.MiddleName)});
                }

                return result.AsQueryable();

            }

        }



        public Employee GetEmployee(long id)
        {
            var filter = String.Format("ID~eq~'{0}'", id.ToString());
            return GetEmployees(filter).FirstOrDefault();
        }

        
        public Employee GetEmployee(string login)
        {
           
            var filter= String.Format("Login~eq~'{0}'", login);
            return GetEmployees(filter).FirstOrDefault();
        }

       

        public IQueryable<TreeViewEmployee> GetEmployeesWithDepartments(string filter)
        {
            using (var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
            {
                UriBuilder builder = new UriBuilder(api_path + "USERS/getUsersWithDeps");
                var parameters = new Dictionary<string, string>
                            {
                               { "sort", "code-asc" },
                               //{ "page", "1" },
                               //{ "pageSize","50" },
                               { "group",""},
                               { "filter", filter}
                            };
                var content = new FormUrlEncodedContent(parameters);
                var response = client.PostAsync(builder.Uri, content).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;
                dynamic obj = JsonConvert.DeserializeObject(responseString);
                List<TreeViewEmployee> result = new List<TreeViewEmployee>();
                foreach (var employee in obj.Data)
                {
                    result.Add(new TreeViewEmployee { ID = employee.id, Name = employee.name, Code = employee.code , hasChildren=employee.hasChildren, ParentId= employee.parentID });
                }

                return result.AsQueryable();

            }
        }
    }
}
