using System;
using System.IO;
using System.Web.Mvc;
using ServiceStack.Text;

namespace Web.Helpers
{
    public class ObjectFilter : ActionFilterAttribute 
    {
        public string Param { get; set; }
        public Type RootType { get; set; }

        #region IActionFilter Members

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if ((filterContext.HttpContext.Request.ContentType ?? string.Empty)
                .Contains("application/json"))
            {
                var o = JsonSerializer.DeserializeFromStream(RootType,filterContext.HttpContext.Request.InputStream);
                filterContext.ActionParameters[Param] = o;
            }
            else
            {
                var stream = new StreamReader(filterContext.HttpContext.Request.InputStream,
                    filterContext.HttpContext.Request.ContentEncoding);

                var o = XmlSerializer.DeserializeFromString(stream.ReadToEnd(), RootType);
                filterContext.ActionParameters[Param] = o;
            }
        }

        #endregion
    }
}