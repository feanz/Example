using System.Collections.Generic;
using Example.Core.Model;

namespace Web.ViewModels
{
    public class BookmarksByTagViewModel
    {
        public BookmarksByTagViewModel(string tag, List<Bookmark> bookmarks)
        {
            Tag = tag;
            Bookmarks = bookmarks;
        }

        public string Tag { get; set; }
        public List<Bookmark> Bookmarks { get; private set; }
    }
}