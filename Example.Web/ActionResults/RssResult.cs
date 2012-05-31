using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;

namespace Web.ActionResults
{

    public class RSSResult : ActionResult
    {
        public SyndicationFeed Feed { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/rss+xml";

            var rssFormatter = new Rss20FeedFormatter(Feed);
            using (var writer = XmlWriter.Create(context.HttpContext.Response.Output))
            {
                rssFormatter.WriteTo(writer);
            }
        }
    }

    //Example RSS
    //public ActionResult Feed( int? page, int? pageSize ) 
    //{
    //  List<PostModel> posts = new List<PostModel>();
    //  Uri site = new Uri(Engine.GetWebAppRoot());

    //  SyndicationFeed feed = new SyndicationFeed("SiteName", "SiteDescription",  Url.SiteRoot() );
    //  List<SyndicationItem> items = new List<SyndicationItem>();
    //  foreach( PostModel post in posts ) 
    //  {
    //      SyndicationItem item = new SyndicationItem(post.Title, post.Content,
    //          new Uri( Url.SiteRoot() + "/Posts/" + post.Slug), post.ID.ToString(), post.PublishDate ?? DateTime.Now);
    //      items.Add(item);
    //  }
    //  feed.Items = items;
    //  return new RSSResult() { Feed = feed };
}
