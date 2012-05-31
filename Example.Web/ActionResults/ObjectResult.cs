using System;
using System.IO;
using System.Text;
using System.Web.Mvc;
using ServiceStack.Text;

namespace Web.ActionResults
{
    public class ObjectResult<T> : ActionResult
    {
        private static UTF8Encoding _utf8Encoding = new UTF8Encoding(false);

        public T Data { get; set; }

        public Type[] IncludedTypes = new[] { typeof(object) };

        public ObjectResult(T data)
        {
            Data = data;
        }

        public ObjectResult(T data, Type[] extraTypes)
        {
            Data = data;
            IncludedTypes = extraTypes;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            // If ContentType is not expected to be application/json, then return XML
            if ((context.HttpContext.Request.ContentType ?? string.Empty).Contains("application/json"))
            {
                new JsonResult { Data = Data ,JsonRequestBehavior = JsonRequestBehavior.AllowGet}
                    .ExecuteResult(context);
            }
            else
            {
                 using (var memoryStream = new MemoryStream())
                 {
                     XmlSerializer.SerializeToStream(Data, memoryStream);
                     new ContentResult
                         {
                             ContentType = "text/xml",
                             Content = _utf8Encoding.GetString(memoryStream.ToArray()),
                             ContentEncoding = _utf8Encoding
                         }
                         .ExecuteResult(context);
                 }
                // NOTE: We could cache XmlSerializer for specific type. Probably use the 
                // GenerateSerializer to generate compiled custom made serializer for specific
                // types and then cache the reference
            }
        }

    }
}