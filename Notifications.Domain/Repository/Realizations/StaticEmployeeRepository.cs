using System;
using System.Collections.Generic;
using System.Linq;
using Notifications.Domain.Entities;
using Notifications.Domain.Repository.Abstract;

namespace Notifications.Domain.Repository.Real
{
    public class StaticEmployeeRepository :  IEmployeeRepository
    {
        private List<Employee> employees;
        public StaticEmployeeRepository()
        {
            employees = new List<Employee> {
                                                        new Employee { ID=1, Login="007OreshkinYV", FullName="Орешкин Ю.В." },
                                                        new Employee { ID=2, Login="007BorisovAV", FullName="Борисов А.В." },
                                                        new Employee { ID=3, Login="007LinVL", FullName="Лин Вадим Леонидович" }
                                                   };
        }


        public IQueryable<Employee> Employees
        {
            get
            {
               
                return employees.AsQueryable();
            }
        }

        public IQueryable<Department> GetDepartments()
        {
            throw new NotImplementedException();
        }

        public Employee GetEmployee(long id)
        {
            return employees.FirstOrDefault(e => e.ID == id);
        }

        public Employee GetEmployee(string login)
        {
            return employees.FirstOrDefault(e => e.Login == login);
        }

        public IQueryable<Employee> GetEmployees(string filter)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Employee> GetEmployeesByDepartment(string code)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TreeViewEmployee> GetTreeViewEmployees(string filter)
        {
            throw new NotImplementedException();
        }
    }
}
