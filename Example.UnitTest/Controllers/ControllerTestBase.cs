using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Example;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Web;

namespace Example.UnitTest.Controllers
{
    [TestClass]
    public class ControllerTestBase
    {
        protected static void SetupControllerContext(Controller controller,string[] userRoles = null, NameValueCollection form = null)
        {
            var routes = new RouteCollection();
            MvcApplication.RegisterRoutes(routes);

            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            request.SetupGet(x => x.ApplicationPath).Returns("/");
            request.SetupGet(x => x.Url).Returns(new Uri("http://localhost/a", UriKind.Absolute));
            request.SetupGet(x => x.ServerVariables).Returns(new System.Collections.Specialized.NameValueCollection());
            request.SetupGet(x => x.Form).Returns(new NameValueCollection());
            request.SetupGet(x => x.IsAuthenticated).Returns(true);

            var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            response.Setup(x => x.ApplyAppPathModifier("/post1")).Returns("http://localhost/post1");

            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);
            context.SetupGet(x => x.Items).Returns(new Dictionary<string, string>());
            context.SetupGet(x => x.User.Identity.Name).Returns("someUser");
            context.SetupGet(x => x.Request.IsAuthenticated).Returns(true);
            context.Setup(x => x.User.IsInRole("User")).Returns(() => true);
            context.Setup(x => x.Request).Returns(request.Object);
            if (form != null)
                context.Setup(x => x.Request.Form).Returns(form);

            //Add user roles to contexts User
            if (userRoles != null)
            {
                foreach (var role in userRoles)
                {
                    context.Setup(x => x.User.IsInRole(role)).Returns(true);
                }
            }

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);            
            controller.Url = new UrlHelper(new RequestContext(context.Object, new RouteData()), routes);
        }
    }
}
