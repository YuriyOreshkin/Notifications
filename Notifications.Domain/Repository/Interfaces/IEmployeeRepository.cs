using Notifications.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Domain.Repository.Interfaces
{
    public interface IEmployeeRepository
    {
        IQueryable<Employee> GetEmployees(string filter);
        Employee GetEmployee(long id);
        Employee GetEmployee(string login);

        IQueryable<TreeViewEmployee> GetEmployeesWithDepartments(string filter);
    }

}
