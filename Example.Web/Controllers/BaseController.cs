using System.Text;
using System.Web;
using System.Web.Mvc;
using NHibernate;
using Ninject;
using ServiceStack.Text;

namespace Web.Controllers
{
    public class BaseController : Controller
    {
        public HttpSessionStateBase HttpSession
        {
            get { return base.Session; }
        }

        [Inject]
        public new ISession Session { get; set; }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new ServiceStackJsonResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding
            };
        }

        public class ServiceStackJsonResult : JsonResult
        {
            public override void ExecuteResult(ControllerContext context)
            {
                var response = context.HttpContext.Response;
                response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

                if (ContentEncoding != null)
                {
                    response.ContentEncoding = ContentEncoding;
                }

                if (Data != null)
                {
                    response.Write(JsonSerializer.SerializeToString(Data));
                }
            }
        }
    }
}
