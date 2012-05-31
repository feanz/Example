using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Example.Core.Model;
using Example.Data.Repositories;
using Example.Data.Repositories.Interfaces;
using Utilities.Extensions;
using Web.ActionResults;
using Web.Areas.Api.Models;
using Web.Controllers;
using Web.Helpers;

namespace Web.Areas.Api.Controllers
{
    public class BookmarksController : BaseController
    {
        private readonly IBookmarkRepository _repository;

        public BookmarksController(IBookmarkRepository repository)
        {
            _repository = repository;
        }

        [RestHttpVerbFilter]
        [Route("Api/Bookmarks/Bookmark/{id?}")]
        public ActionResult Bookmark(int? id, BookmarkDTO item, string httpVerb)
        {
            try
            {
                Bookmark bookmark;
                switch (httpVerb)
                {
                    case "POST":
                        bookmark = Mapper.Map<BookmarkDTO, Bookmark>(item);
                        _repository.InsertOrUpdate(bookmark);
                        return
                            new ObjectResult<RestResult>(new RestResult(0, "SUCCESS",
                                                                        Mapper.Map<Bookmark, BookmarkDTO>(bookmark)));
                    case "PUT":
                        bookmark = Mapper.Map<BookmarkDTO, Bookmark>(item);
                        _repository.InsertOrUpdate(bookmark);
                        return
                            new ObjectResult<RestResult>(new RestResult(0, "SUCCESS",
                                                                        Mapper.Map<Bookmark, BookmarkDTO>(bookmark)));
                    case "GET":
                        var bookmarkDto = Mapper.Map<Bookmark, BookmarkDTO>(_repository.GetById(id.GetValueOrDefault()));
                        return new ObjectResult<BookmarkDTO>(bookmarkDto);
                    case "DELETE":
                        _repository.Delete(id.GetValueOrDefault());
                        return new ObjectResult<RestResult>(new RestResult(0, "SUCCESS", item));
                }
                return new ObjectResult<RestResult>(new RestResult(1, "Unknown HTTP verb", item));
            }
            catch (Exception ex)
            {
                return new ObjectResult<RestResult>(new RestResult(1, ex.Message, ex));
            }
        }

        [HttpGet]
        [Route("Api/Bookmarks/{rows?:INT}/{page?:INT}")]
        public ActionResult BookmarkList(int? rows, int? page)
        {
            try
            {
                IEnumerable<Bookmark> bookmarks;
                if (rows.IsNotNull() && page.IsNotNull())
                {
                    bookmarks = _repository.GetPaged((int) rows, (int) page, null, null);
                    return new ObjectResult<IEnumerable<Bookmark>>(bookmarks);
                }
                bookmarks = _repository.GetPaged(20, 1, null, null);
                return new ObjectResult<IEnumerable<Bookmark>>(bookmarks);
            }
            catch (Exception ex)
            {
                return new ObjectResult<RestResult>(new RestResult(1, ex.Message, ex));
            }
        }

        [Route("Api/Bookmarks")]
        public ActionResult Bookmarks(int? rows, int? page)
        {
            try
            {
                return
                    new ObjectResult<IEnumerable<BookmarkDTO>>(
                        Mapper.Map<IEnumerable<Bookmark>, IEnumerable<BookmarkDTO>>(_repository.GetAll()));
            }
            catch (Exception ex)
            {
                return new ObjectResult<RestResult>(new RestResult(1, ex.Message, ex));
            }
        }

        [HttpPost]
        [Route("Api/Bookmarks")]
        public ActionResult Bookmarks(List<Bookmark> items)
        {
            try
            {
                _repository.InsertOrUpdate(items);
                return new ObjectResult<RestResult>(new RestResult(0, "SUCCESS", items));
            }
            catch (Exception ex)
            {
                return new ObjectResult<RestResult>(new RestResult(1, ex.Message, ex));
            }
        }
    }
}