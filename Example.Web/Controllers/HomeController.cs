using System.Web.Mvc;
using Web.Helpers;

namespace Web.Controllers
{
    [CustomAuthorize(Roles = "User")]
    public class HomeController : BaseController
    {
        public HomeController()
        { }

        [OutputCache(Duration = 60)]
        public ActionResult Index()
        {           
            ViewBag.Message = "Modify this template to kick-start your application.";            
            
            return View();
        }

        [Route("About")]        
        public ActionResult About()
        {
            ViewBag.Message = "Your quintessential app description page.";

            return View();
        }

        [Route("Contact")]        
        public ActionResult Contact()
        {
            ViewBag.Message = "Your quintessential contact page.";

            return View();
        }
    }
}
