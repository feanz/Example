using System;
using System.Web;
using System.Web.Mvc;
using Utilities.Extensions;
using Web.Helpers;

namespace Web.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class ElmahController : Controller
    {
        public ActionResult Index()
        {
            return new ElmahResult();
        }

        public ActionResult Stylesheet()
        {
            return new ElmahResult("stylesheet");
        }

        public ActionResult Rss()
        {
            return new ElmahResult("rss");
        }

        public ActionResult DigestRss()
        {
            return new ElmahResult("digestrss");
        }

        public ActionResult About()
        {
            return new ElmahResult("about");
        }

        public ActionResult Detail()
        {
            return new ElmahResult("detail");
        }

        public ActionResult Download()
        {
            return new ElmahResult("download");
        }

        public ActionResult Json()
        {
            return new ElmahResult("json");
        }

        public ActionResult Xml()
        {
            return new ElmahResult("xml");
        }
    }

    internal class ElmahResult : ActionResult
    {
        private readonly string _resouceType;

        public ElmahResult()
            : this(null)
        {

        }

        public ElmahResult(string resouceType)
        {
            _resouceType = resouceType;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var factory = new Elmah.ErrorLogPageFactory();

            if (!string.IsNullOrEmpty(_resouceType))
            {
                var pathInfo = "/" + _resouceType;
                context.HttpContext.RewritePath(FilePath(context), pathInfo, context.HttpContext.Request.QueryString.ToStringCurrentCulture());
            }

            var currentContext = GetCurrentContext(context);

            var httpHandler = factory.GetHandler(currentContext, null, null, null);
            var httpAsyncHandler = httpHandler as IHttpAsyncHandler;

            if (httpAsyncHandler != null)
            {
                httpAsyncHandler.BeginProcessRequest(currentContext, r => { }, null);
                return;
            }

            httpHandler.ProcessRequest(currentContext);
        }

        private static HttpContext GetCurrentContext(ControllerContext context)
        {
            var currentApplication = (HttpApplication)context.HttpContext.GetService(typeof(HttpApplication));
            return currentApplication.Context;
        }

        private string FilePath(ControllerContext context)
        {
            return _resouceType != "stylesheet" ?
                context.HttpContext.Request.Path.Replace(String.Format("/{0}", _resouceType), string.Empty) : context.HttpContext.Request.Path;
        }
    }
}
