using Notifications.WebUI.App_Start;
using Notifications.WebUI.Models;
using System.Web.Mvc;

namespace Notifications.WebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
      
        public ActionResult Index()
        {
            return View();
        }

       

        public ActionResult Administration()
        {
           

            return View();
        }

    }
}