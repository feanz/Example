using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Example.Core.Model;
using Utilities.Extensions;

namespace Web.Models
{
    public class BookmarkModelBinder : DefaultModelBinder
    {
        protected override void OnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var form = controllerContext.HttpContext.Request.Form;
            var tagsAsString = form["TagsAsString"];
            var typeId = form["Type.Id"];
            var bookmark = bindingContext.Model as Bookmark;
            if (typeId.IsNullOrEmpty())
            {
                bookmark.BookmarkType = new BookmarkType()
                                            {
                                                Id = int.Parse(typeId, CultureInfo.CurrentCulture),
                                                TypeName = string.Empty
                                            };
                bookmark.SetTags(tagsAsString.IsNullOrEmpty()
                                     ? new List<string>()
                                     : tagsAsString.Split(',').Select(i => i.Trim()).ToList());
            }
        }
    }
}