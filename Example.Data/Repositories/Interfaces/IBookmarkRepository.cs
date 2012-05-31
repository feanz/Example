using System.Collections.Generic;
using Example.Core.Model;

namespace Example.Data.Repositories.Interfaces
{
    public interface IBookmarkRepository : IGenericRepository<Bookmark>
    {
        IEnumerable<BookmarkType> GetTypes();
        IEnumerable<string> GetTags(string tagName);
    }
}
