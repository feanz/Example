using System.Collections.Generic;

namespace Example.Core.Model
{
    public class BookmarkType
    {
        public virtual int Id { get; set; }

        public virtual string TypeName { get; set; }

        public virtual IList<Bookmark> Bookmarks { get; set; }
    }
}
