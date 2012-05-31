using FluentNHibernate.Mapping;
using Example.Core.Model;

namespace Web.Persistence.DataMapping
{
    public class BookmarkMap : ClassMap<Bookmark>
    {
        public BookmarkMap()
        {
            Id(x => x.Id);
            Map(x => x.Title);
            Map(x => x.Url);
            Map(x => x.Description);
            Map(x => x.DateStarted);
            Map(x => x.DateClosed);            
            Map(x => x.Tags).CustomType(typeof(string)).Access.CamelCaseField(Prefix.Underscore);//Tags are storede as a comma delimted string
            References(x => x.BookmarkType).Column("TYPE_ID").Not.LazyLoad().Fetch.Join();//Makes sure the type is aloways loaded
        }
    }
}