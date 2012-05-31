using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Example.Core.Model;
using Example.Data.Repositories.Interfaces;
using NHibernate;
using NHibernate.Linq;
using Utilities.Extensions;

namespace Example.Data.Repositories
{
    public class BookmarkRepository : GenericRepository<Bookmark>, IBookmarkRepository
    {
        public BookmarkRepository()
            : this(null)
        {
        }

        public BookmarkRepository(ISession session)
            : base(session)
        {
        }

        public override IEnumerable<Bookmark> GetBy(System.Linq.Expressions.Expression<System.Func<Bookmark, bool>> expression)
        {
            return Session.Query<Bookmark>().Fetch(x =>x.BookmarkType).Where(expression);
        }

        public IEnumerable<BookmarkType> GetTypes()
        {
            return Session.Query<BookmarkType>().ToList();
        }

        public virtual IEnumerable<string> GetTags(string tagName)
        {
            const string queryString = "SELECT Tags FROM Bookmark where upper(Tags) LIKE :tagName";
            var query = Session.CreateQuery(queryString)
                   .SetParameter("tagName", '%' + tagName.ToUpperInvariant() + '%');                   

            var list = new List<string>();            
            
            foreach(var item in query.List())
            {
                list.Concat(item.ToString().ToList().Where(x => x.ContainsCaseInsensitive(tagName)));                
            }
            return list;
        }
    }
}
