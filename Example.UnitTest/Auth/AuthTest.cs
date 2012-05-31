using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareApproach.TestingExtensions;
using Test.Mocks;

namespace Example.UnitTest.Auth
{
    /// <summary>
    /// Test the custom NuthAuthorizeAttribute  
    /// </summary>
	[TestClass]
	public class AuthTest
	{
		[TestMethod]
		public void User_Should_Be_Authorize_And_Execute_The_Action()
		{
			var controller = new FooController();

            //Fake Auth Context for User
			controller.SetFakeAuthenticatedControllerContext("someUser",false);

            //Call an action that requires user permissions 
			Assert.IsTrue(new ActionInvokerExpecter<ViewResult>()
				.InvokeAction(controller.ControllerContext, "UserAction"));
		}

        [TestMethod]
        public void User_Should_Not_Be_Authorize_the_Action_Execution()
        {
            var controller = new FooController();

            //Fake Auth Context for User
            controller.SetFakeAuthenticatedControllerContext("someUser", false);
            
            //Call an action that requires admin permissions, should redirect user
            Assert.IsTrue(new ActionInvokerExpecter<RedirectToRouteResult>().InvokeAction(controller.ControllerContext, "AdminAction"));
        }

		[TestMethod]
		public void Admin_Should_Be_Authorize_the_User_Action_Execution()
		{
			var controller = new FooController();

            //Fake Auth Context for Admin
			controller.SetFakeAuthenticatedControllerContext("someUser",true);

            //Call an action that requires user permissions (Admins can invoke all actions regardless of weather they are flagged for admins)
            Assert.IsTrue(new ActionInvokerExpecter<ViewResult>()
                .InvokeAction(controller.ControllerContext, "UserAction"));
		}

        [TestMethod]
        public void Admin_Should_Be_Authorize_the_Admin_Action_Execution()
        {
            var controller = new FooController();

            //Fake Auth Context for Admin
            controller.SetFakeAuthenticatedControllerContext("someUser", true);

            //Call an action that requires admin permissions 
            Assert.IsTrue(new ActionInvokerExpecter<ViewResult>()
                .InvokeAction(controller.ControllerContext, "AdminAction"));
        }
	}
}