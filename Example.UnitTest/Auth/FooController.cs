using System.Web.Mvc;
using Web.Helpers;

namespace Example.UnitTest.Auth
{
	public class FooController : Controller
	{
        [CustomAuthorize(Roles = "Admin")] //Need Admin authorization level to get executed.
        public ActionResult AdminAction()
		{
			return View();
		}

        [CustomAuthorize(Roles = "User")] //User level auth
        public ActionResult UserAction()
		{
			return View();
		}
	}
}