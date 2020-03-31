using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Notifications.Domain.Repository.Interfaces;
using Notifications.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Notifications.WebUI.Controllers
{
    public class EmployeeDirectoryController : Controller
    {
        private IEmployeeRepository repository;

        public EmployeeDirectoryController(IEmployeeRepository _repository)
        {
            repository = _repository;
        }

        public JsonResult ReadEmployees(string text)
        {
            var filter = "";
           
                if (!string.IsNullOrEmpty(text))
                {
                    filter = String.Format("FIO~contains~'{0}'", text);
                }

                var employees = repository.GetEmployees(filter).Select(e => new EmployeeViewModel
                {
                    id = e.ID,
                    fullname = e.FullName

                });

                return Json(employees, JsonRequestBehavior.AllowGet);
        }


        private long[] GetParents(long? parentId)
        {
            var result =new List<long>();

            while (parentId != null)
            {
                result.Add((long)parentId);
                var filter = String.Format("ID~eq~'{0}'", parentId);
                parentId = repository.GetEmployeesWithDepartments(filter).FirstOrDefault().ParentId;
             }

            return result.ToArray();
            
        }

        public JsonResult ReadEmployeesTreeView(long? id,string text,long[] values)
        {
            var filter =  id.HasValue ? String.Format("parentID~eq~'{0}'", id.ToString()): !string.IsNullOrEmpty(text) ? String.Format("name~contains~'{0}'", text) : String.Format("parentID~isnull~null");

            var employees = repository.GetEmployeesWithDepartments(filter).Select(e => new EmployeeTreeViewModel
            {
                id = e.ID,
                fullname = e.Name,
                hasChildren = e.hasChildren
                //parents=GetParents(e.parentID)
            });

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

    }
}