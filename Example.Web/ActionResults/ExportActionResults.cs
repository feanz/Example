using System;
using System.Web.Mvc;
using System.Web;
using System.Collections;

namespace Web.ActionResults
{
    public abstract class ExportActionResultBase : ActionResult
    {
        public IEnumerable Records { get; protected set; }
        public string[] ColumnHeaders { get; protected set; }
        public string FileName { get; protected set; }

        protected static Type GetType(IEnumerable records)
        {
            var enumerator = records.GetEnumerator();
            if (!enumerator.MoveNext() || enumerator.Current == null)
                return typeof(object);
            return enumerator.Current.GetType();
        }

        protected static string ReplaceSpecialCharacters(string value)
        {
            value = value.Replace("’", "'");
            value = value.Replace("“", "\"");
            value = value.Replace("”", "\"");
            value = value.Replace("–", "-");
            value = value.Replace("…", "...");
            value = value.Replace("\"", "\"\"");
            return value;
        }

        protected static void WriteFile(string fileName, string contentType, string content)
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                context.Response.Clear();
                if (!string.IsNullOrEmpty(fileName))
                    context.Response.AddHeader("content-disposition", "attachment;filename=" + fileName.Replace(" ", "_"));

                context.Response.AddHeader("Cache-Control", "must-revalidate");
                context.Response.AddHeader("Pragma", "public");
                context.Response.ExpiresAbsolute = DateTime.FromOADate(DateTime.Now.ToOADate() - 1);

                context.Response.Charset = "";
                context.Response.ContentType = contentType;
                context.Response.Write(content);
                context.Response.End();
            }
        }
    }
}
