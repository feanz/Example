using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using System.Configuration;

namespace System.Web.Mvc
{
    public static class HtmlHelpers
    {
        const string _imageDir = "Images";

        /// <summary>
        /// Returns true if we are currently in a dev enviroment.  
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static bool IsDev(this HtmlHelper helper)
        {
            return ConfigurationManager.AppSettings["Environment"].ToString(CultureInfo.InvariantCulture) == "Dev";
        }

        /// <summary>
        /// Get the current enviroment setting
        /// </summary>
        /// <returns></returns>
        public static IHtmlString GetEnvironment(this HtmlHelper helper)
        {
            var environment = string.Empty;
            if (ConfigurationManager.AppSettings["Environment"].ToString(CultureInfo.InvariantCulture).ToUpperInvariant() != "PROD")
                environment = "( " + ConfigurationManager.AppSettings["Environment"].ToString(CultureInfo.InvariantCulture) + " )";

            return new HtmlString(environment);
        }

        /// <summary>
        /// Get the current application version number
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static IHtmlString GetApplicationVersionID(this HtmlHelper helper)
        {
            return new HtmlString(Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }

        /// <summary>
        /// Set all .date-selector elements on page as jquery UI date pickers
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static IHtmlString DatePickerEnable(this HtmlHelper helper)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<script type='text/javascript'>$(document).ready(function() {$('.date-selector').datepicker({ changeMonth: true, changeYear: true, dateFormat: 'dd/mm/yy', yearRange: '-100:+1' });});</script>\n");
            return new HtmlString(sb.ToString());
        }

        /// <summary>
        /// Output checkbox items for supplied IEnumerable SelectListItem
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IHtmlString CheckBoxList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> items)
        {
            var output = new StringBuilder();
            output.Append(@"<div class=""checkboxList"">");

            foreach (var item in items)
            {
                output.Append(@"<input type=""checkbox"" name=""");
                output.Append(name);
                output.Append("\" value=\"");
                output.Append(item.Value);
                output.Append("\"");

                if (item.Selected)
                    output.Append(@" checked=""chekced""");

                output.Append(" />");
                output.Append(item.Text);
                output.Append("<br />");
            }

            output.Append("</div>");

            return new HtmlString(output.ToString());
        }

        /// <summary>
        /// Html Image tag for supplied filename 
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static IHtmlString Image(this HtmlHelper helper, string fileName)
        {
            return Image(helper, fileName, "");
        }

        /// <summary>
        /// Html Image tag for supplied filename and attribute collection
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="fileName"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static IHtmlString Image(this HtmlHelper helper, string fileName, string attributes)
        {
            var path = VirtualPathUtility.ToAbsolute(string.Format("~/{0}/{1}", _imageDir, fileName));
            return new HtmlString(string.Format("<img src='{0}' {1} />", helper.AttributeEncode(path), attributes));
        }

        /// <summary>
        ///  Html Link Image tag for supplied filename
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="fileName"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static IHtmlString ImageLink(this HtmlHelper helper, string fileName, string link)
        {
            return ImageLink(helper, fileName, link, "");
        }

        /// <summary>
        /// Html Link Image tag for supplied filename and attribute collection
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="fileName"></param>
        /// <param name="link"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static IHtmlString ImageLink(this HtmlHelper helper, string fileName, string link, string attributes)
        {
            var path = VirtualPathUtility.ToAbsolute(string.Format("~/{0}/{1}", _imageDir, fileName));
            return new HtmlString(string.Format("<a href='{0}'><img src='{1}' {2} /></a>", link, helper.AttributeEncode(path), attributes));
        }

        /// <summary>
        /// Output Menu UL based on the contents of default Sitemap. Menu supports permission based filtering 
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="user"></param>
        /// <param name="menuItem"></param>
        /// <returns></returns>
        public static IHtmlString Menu(this HtmlHelper helper, IPrincipal user, string menuItem = "")
        {
            var sb = new StringBuilder();

            // Create opening unordered items tag
            sb.Append("<ul id='menu'>");

            // Render each top level node            
            var topLevelNodes = SiteMap.RootNode.ChildNodes;
            var count = 1;
            foreach (SiteMapNode node in topLevelNodes)
            {
                //If the user has permissions to see this menu item or they are a developer (developers see all)
                if ((node.Roles.Count == 0) || user.IsInRole(node.Roles[0].ToString()) || user.IsInRole("Admin"))
                {
                    sb.AppendLine("<li>");

                    //If a menu item has been provided check if the node contains the menuitem and mark as selected
                    if (!string.IsNullOrEmpty(menuItem))
                    {
                        sb.AppendFormat(
                            node.Title == menuItem
                                ? "<a href='{0}' class='selectedMenuItem'>{1}</a>"
                                : "<a href='{0}'>{1}</a>", node.Url, helper.Encode(node.Title));
                    }
                    //If there is no current node make first node selected
                    else if (SiteMap.CurrentNode == null && count == 1)
                        sb.AppendFormat("<a href='{0}' class='selectedMenuItem'>{1}</a>", node.Url, helper.Encode(node.Title));
                    else
                        sb.AppendFormat("<a href='{0}'>{1}</a>", node.Url, helper.Encode(node.Title));

                    sb.AppendLine("</li>");
                }

                count++;
            }

            // Close unordered items tag
            sb.Append("</ul>");

            return new HtmlString(sb.ToString());
        }
    }
}
