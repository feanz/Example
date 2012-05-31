using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace Test.Mocks
{
	public static class MvcMocks
	{
        /// <summary>
        /// Create a complete HttpContext
        /// </summary>
        /// <param name="username">The username of the HttpContext.User created </param>
        /// <param name="isAdmin">Set the user to admin if true</param>
        /// <returns></returns>
		public static HttpContextBase FakeAuthenticatedHttpContext(string username,bool isAdmin)
		{
			var context = new Mock<HttpContextBase>();
			var request = new Mock<HttpRequestBase>();
			var response = new Mock<HttpResponseBase>();
			var session = new Mock<HttpSessionStateBase>();
			var server = new Mock<HttpServerUtilityBase>();
			var user = new Mock<IPrincipal>();
			var identity = new Mock<IIdentity>();

			context.Setup(ctx => ctx.Request).Returns(request.Object);
			context.Setup(ctx => ctx.Response).Returns(response.Object);
			context.Setup(ctx => ctx.Session).Returns(session.Object);
			context.Setup(ctx => ctx.Server).Returns(server.Object);
			context.Setup(ctx => ctx.User).Returns(user.Object);
            context.Setup(ctx => ctx.User.Identity.IsAuthenticated).Returns(true);                        
            context.Setup(ctx => ctx.User.IsInRole("User")).Returns(true);

            if(isAdmin)
                context.Setup(ctx => ctx.User.IsInRole("Admin")).Returns(true);

			user.Setup(ctx => ctx.Identity).Returns(identity.Object);
			identity.Setup(id => id.IsAuthenticated).Returns(true);            
			identity.Setup(id => id.Name).Returns(username);
			context.Setup(ctx => ctx.Response.Cache).Returns(CreateCachePolicy());
			return context.Object;
		}

		public static HttpCachePolicyBase CreateCachePolicy()
		{
			var mock = new Mock<HttpCachePolicyBase>();
			return mock.Object;
		}

		public static void SetFakeAuthenticatedControllerContext(this Controller controller, string username,bool isAdmin)
		{
            HttpContextBase httpContext = FakeAuthenticatedHttpContext(username, isAdmin);
			var context = new ControllerContext(new RequestContext(httpContext, new RouteData()), controller);
			controller.ControllerContext = context;
		}
	}
}