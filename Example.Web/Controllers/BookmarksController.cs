using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using AutoMapper;
using Example.Core.Model;
using Example.Data.Repositories;
using Example.Data.Repositories.Interfaces;
using Microsoft.Security.Application;
using Telerik.Web.Mvc;
using Web.Helpers;
using Web.ViewModels;
using Utilities.Extensions;

namespace Web.Controllers
{
    public class BookmarksController : BaseController
    {
        private readonly IBookmarkRepository _repository;

        public BookmarksController(IBookmarkRepository repository)
        {
            _repository = repository;
        }

        public ViewResult Index()
        {
            var model = _repository.GetAll();

            return View(model);
        }

        public ViewResult Details(int id)
        {
            var bookmark = _repository.GetById(id);            

            var viewModel = Mapper.Map<BookmarkViewModel>(bookmark);

            return View(viewModel);
        }

        public ActionResult Create()
        {
            ViewBag.Types = new SelectList(_repository.GetTypes(), "Id", "TypeName");

            var model = new Bookmark();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken,ValidateInput(false)]
        public ActionResult Create(Bookmark bookmark)
        {
            if (ModelState.IsValid)
            {
                bookmark.Description = Encoder.HtmlEncode(bookmark.Description);

                _repository.InsertOrUpdate(bookmark);
                return RedirectToAction("Index");
            }
            ViewBag.Types = new SelectList(_repository.GetTypes(), "Id", "TypeName");
            return View(bookmark);
        }
        
        public ActionResult Edit(int id)
        {
            var bookmark = _repository.GetById(id);

            if (bookmark.IsNotNull())
            {
                ViewBag.Types = new SelectList(_repository.GetTypes(), "Id", "TypeName");
                return View(bookmark);
            }
            return new ResourceNotFoundResult("Bookmark does not exist");
        }
        
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(Bookmark bookmark)
        {
            if (ModelState.IsValid)
            {
                _repository.InsertOrUpdate(bookmark);
                return RedirectToAction("Index");
            }
            ViewBag.Types = new SelectList(_repository.GetTypes(), "Id", "TypeName");
            return View(bookmark);
        }

        public ActionResult Delete(int id)
        {
            var model = _repository.GetById(id);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            _repository.Delete(id);
            return RedirectToAction("Index");
        }

        [Route("Bookmarks/Tag/{tag?}")]
        public ViewResult Tag(string tag)
        {
            var bookmarks = _repository.GetAll()
                    .Where(i => i.Tags.Any(t => t == tag))
                    .OrderByDescending(i => i.DateStarted)
                    .ToList();

            var model = new BookmarksByTagViewModel(tag, bookmarks);

            return View(model);
        }

        public ViewResult BackBone()
        {
            return View();
        }

        public ViewResult BookmarkTemplate()
        {
            return View();
        }

        public ActionResult TelerikGrid()
        {
            var model = _repository.GetAll();

            return View(model);
        }

        [GridAction]
        public ActionResult _TelerikGrid()
        {
            var model = _repository.GetAll();

            return View(new GridModel(model));
        }

        public ViewResult BookmarkKnockoutJs()
        {
            return View();
        }

        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetBookmarks(int rows, int page, string sidx, string sord)
        {
            var count = _repository.GetCount();
            var bookmarks = _repository.GetPaged(rows, page, sidx, sord);
            return Json(new
            {
                page = page,
                records = count,
                rows = bookmarks,
                total = Math.Ceiling((decimal)count / rows)
            }, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetAllBookmarks()
        {
            var bookmarks = _repository.GetAll();
            return Json(bookmarks, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Savebookmark()
        {
            return Json(false,JsonRequestBehavior.DenyGet);
        }

        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetBookmark(string title)
        {
            var bookmark = _repository.GetBy(x => x.Title == title).FirstOrDefault();
            
            if (bookmark.IsNotNull())
                return Json(bookmark, JsonRequestBehavior.AllowGet);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetTags(string term)
        {
            //Convert tag to items 
            var list = term.ToList();

            //take last item from items and trim it
            var tagName = list.Last().Trim();

            var tags = _repository.GetTags(tagName);
            if (tags.IsNotNull())            
                return Json(tags, JsonRequestBehavior.AllowGet);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}