using System.Web.Mvc;

namespace Web.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Common()
        {
            return View();
        }
    }
}
