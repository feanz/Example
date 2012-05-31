using FluentNHibernate.Mapping;
using Example.Core.Model;

namespace Web.Persistence.DataMapping
{
    public class BookmarkTypeMap : ClassMap<BookmarkType>
    {
        public BookmarkTypeMap()
        {
            Id(x => x.Id);
            Map(x => x.TypeName);          
        }
    }
}