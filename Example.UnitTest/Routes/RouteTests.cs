using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Routing;
using SoftwareApproach.TestingExtensions;
using Web;
using System.Web;
using Moq;
using System.Web.Mvc;
using Example.UnitTest.Fakes;
using Web.Controllers;

namespace Example.UnitTest.Routes
{
    [TestClass]
    public class RouteTests
    {
        [TestInitialize]
        public void Setup()
        {
            RouteTable.Routes.Clear();
            MvcApplication.RegisterRoutes(RouteTable.Routes);            
        }

        #region HomeController
        [TestMethod]
        public void Default_url_should_route_to_home_index()
        {
            "~/".Route().ShouldMapTo<HomeController>(x => x.Index());
        }

        [TestMethod]
        public void About_url_should_route_to_home_about()
        {
            "~/About".Route().ShouldMapTo<HomeController>(x => x.About());
        }

        [TestMethod]
        public void Contact_url_should_route_to_home_contacts()
        {
            "~/Contact".Route().ShouldMapTo<HomeController>(x => x.Contact());
        } 
        #endregion

        #region ErrorController
        [TestMethod]
        public void Error_url_should_route_to_error_problem()
        {
            "~/Error".Route().ShouldMapTo<ErrorController>(x => x.Problem());
        }

        [TestMethod]
        public void NoAccess_url_should_route_to_error_NoAccess()
        {
            "~/NoAccess".Route().ShouldMapTo<ErrorController>(x => x.NoAccess());
        }

        [TestMethod]
        public void InsufficientPermissions_url_should_route_to_error_InsufficientPermissions()
        {
            "~/InsufficientPermissions".Route().ShouldMapTo<ErrorController>(x => x.InsufficientPermissions());
        } 
        #endregion

        #region BooksmarksController
        [TestMethod]
        public void Bookmark_tag_tagid_should_route_to_bookmark_tags()
        {
            "~/Bookmarks/Tag/1".Route().ShouldMapTo<BookmarksController>(x => x.Tag("1"));
        } 
        #endregion
    }
}
