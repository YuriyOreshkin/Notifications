using System.Web.Mvc;

namespace Notifications.WebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
      
        public ActionResult Index()
        {
            ViewBag.Title = "Notifications";
            return View();
        }

       

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

    }
}