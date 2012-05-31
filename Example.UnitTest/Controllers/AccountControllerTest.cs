using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using Example.Core.Model;
using Example.Data.Repositories;
using Example.Data.Repositories.Interfaces;
using Example.UnitTest.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SoftwareApproach.TestingExtensions;
using Web.Controllers;

namespace Example.UnitTest.Controllers
{

    [TestClass]
    public class AccountControllerTest : ControllerTestBase
    {
        FakeData _data = new FakeData();

        AccountController CreateAccountController(IAccountRepository repo = null, NameValueCollection form = null)
        {
            var mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository.Setup(r => r.GetAll()).Returns(new List<User>());           
            
            var controller = new AccountController(repo ?? mockAccountRepository.Object);

            var roles = new string[]{"SuperUser"};
            SetupControllerContext(controller, roles,form);

            return controller;                       
        }

        [TestMethod]
        public void Index_Action_Should_Return_View_Of_Users()
        {
            //Arrange
            var controller = CreateAccountController();

            //Act            
            var result = controller.Index(1);

            //Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(IEnumerable<User>));
        }

        [TestMethod]
        public void Create_Action_Should_Return_User_View()
        {
            //Arrange
            var controller = CreateAccountController();

            //Act
            //1 is a valid patient id on fake repository
            var result = controller.Create() as ViewResult;

            //Assert
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(User));
            Assert.IsNotNull(result.ViewData.Model);
        }

        [TestMethod]
        public void Create_Action_Should_Add_Roles_Data_To_View_Data()
        {
            //Arrange
            var controller = CreateAccountController();

            //Act
            //1 is a valid patient id on fake repository
            var result = controller.Create() as ViewResult;

            //Assert
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(User));
            Assert.IsNotNull(result.ViewBag.PossibleRoles);
        }

        [TestMethod]
        public void Create_Action_Post_Should_Redirect_To_Index_When_Successful()
        {
            // Arrange
            var form = _data.CreateUserNamedValueCollection();
            var controller = CreateAccountController(form: form);
            
            // Act
            var user = _data.CreateUser();            
            var result = controller.Create(user) as RedirectToRouteResult;

            // Assert            
            Assert.AreEqual("Index", result.RouteValues["action"]);            
        }

        [TestMethod]
        public void Create_Action_Post_Should_Return_View_When_UnSuccessful()
        {
            // Arrange
            var controller = CreateAccountController();
            controller.ModelState.AddModelError("An error", "Message");

            // Act
            var user = new User();
            var result = controller.Create(user) as ViewResult;

            // Assert
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(User));
        }

        [TestMethod]
        public void Create_Action_Post_Should_Not_Call_CreateUser_When_Model_Invalide()
        {
            //Arrange    
            var mock = new Mock<IAccountRepository>();

            var controller = CreateAccountController(repo: mock.Object);
            controller.ModelState.AddModelError("Error", "An Error happened");

            //Act
            var user = _data.CreateUser();
            var result = controller.Create(user) as ViewResult;

            //Assert   
            mock.Verify(x => x.InsertOrUpdate(user), Times.Never());
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(User));
        }

        [TestMethod]
        public void Edit_Action_Should_Return_User_View_For_Valid_Username()
        {
            //Arrange 
            var mock = new Mock<IAccountRepository>();
            mock.Setup(m => m.GetById(1)).Returns(_data.CreateUser());

            var controller = CreateAccountController(repo: mock.Object);

            //Act
            var result = controller.Edit(1) as ViewResult;

            //Assert 
            mock.Verify(x => x.GetById(1), Times.Once());
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(User));
            Assert.IsNotNull(result.ViewData.Model);
        }

        [TestMethod]
        public void Edit_Action_Should_Return_ReasourceNotFound_Result_For_InValid_Username()
        {
            //Arrange 
            var mock = new Mock<IAccountRepository>();
            User user = null;
            mock.Setup(m => m.GetById(999)).Returns(user);

            var controller = CreateAccountController(repo: mock.Object);

            //Act
            var result = controller.Edit(999) as ActionResult;

            //Assert 
            Assert.IsInstanceOfType(result, typeof(ResourceNotFoundResult));
        }

        [TestMethod]
        public void Edit_Action_Post_Should_Redirect_To_Index_When_Successful()
        {
            //Arrange 
            var user = _data.CreateUser();
            var form = _data.CreateUserForm();

            var mock = new Mock<IAccountRepository>();
            mock.Setup(m => m.GetById(1)).Returns(user);

            var controller = CreateAccountController(mock.Object);

            //Act            
            controller.ValueProvider = form.ToValueProvider();
            var result = controller.Edit(1, form,new string[0]) as RedirectToRouteResult;

            //Assert 
            mock.Verify(x => x.InsertOrUpdate(user), Times.Once());
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void Edit_Action_Post_Should_Return_View_When_UnSuccessful()
        {
            // Arrange
            var user = _data.CreateUser();
            var form = _data.CreateUserForm();

            var mock = new Mock<IAccountRepository>();
            mock.Setup(m => m.GetById(1)).Returns(user);

            var controller = CreateAccountController(mock.Object);
            //Add model state error
            controller.ModelState.AddModelError("An error", "Message");

            // Act
            controller.ValueProvider = form.ToValueProvider();
            var result = controller.Edit(1, form, new string[0]) as ViewResult;

            // Assert            
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(User));
        }

        [TestMethod]
        public void Edit_Action_Post_Should_Not_CreateUser_When_Model_Invalide()
        {
            //Arrange  
            var user = _data.CreateUser();
            var form = _data.CreateUserForm();

            var mock = new Mock<IAccountRepository>();
            mock.Setup(m => m.GetById(1)).Returns(user);

            var controller = CreateAccountController(mock.Object);
            //Add model state error
            controller.ModelState.AddModelError("Error", "An Error happened");

            //Act
            controller.ValueProvider = form.ToValueProvider();
            var result = controller.Edit(1, form, new string[0]) as ViewResult;

            //Assert   
            mock.Verify(x => x.InsertOrUpdate(user), Times.Never(),"Insert statement was called when there where errors in model");
            result.ViewData.Model.ShouldBeOfType(typeof(User),"Model is not a user");
        }

        [TestMethod]
        public void Verify_Controller_Is_Decorated_With_SuperUser_Authorize_Attribute()
        {
            //Arrange
            var controller = CreateAccountController();

            //Act
            var type = controller.GetType();
            var attribute = type.GetCustomAttributes(typeof(AuthorizeAttribute), true).First() as AuthorizeAttribute;

            //Assert
            Assert.IsTrue(attribute != null, "No AuthorizeAttribute found on AccountContoller");
            Assert.IsTrue(attribute.Roles.Contains("SuperUser"), "AuthorizeAttribute on account controller is not set for superuser role");
        }

        [TestMethod]
        public void Verify_Delete_Method_Is_Decorated_With_Admin_Authorize_Attribute()
        {
            //Arrange
            var controller = CreateAccountController();

            //Act
            var type = controller.GetType();
            var methodInfo = type.GetMethod("Delete");
            var attribute = methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), true).First() as AuthorizeAttribute;

            //Assert            
            Assert.IsTrue(attribute != null, "No AuthorizeAttribute found on Delete(int id) method");
            Assert.IsTrue(attribute.Roles.Contains("Admin"),"AuthorizeAttribute on account controller is not set for admin role");
        }
    }
}
