using System.Collections.Specialized;
using System.Web.Mvc;
using Example.Core.Model;

namespace Example.UnitTest.Fakes
{
    class FakeData
    {
        public FakeData()
        {
        
        }

        public User CreateUser()
        {
            User item = new User();
            item.UserName = "someUser";
            item.FirstName = "firstname";
            item.LastName = "LastName";
            item.Email = "someUser@g4s.com";            

            return item;
        }

        public FormCollection CreateUserForm()
        {
            var form = new FormCollection();
            form.Add("User", "user");
            form.Add("FirtsName", "first");
            form.Add("LastName", "last");
            form.Add("Email", "someUser@g4s.com");
            form.Add("RoleIds", "1,2");

            return form;
        }

        public NameValueCollection CreateUserNamedValueCollection()
        {
            var form = new NameValueCollection();
            form.Add("User", "user");
            form.Add("FirtsName", "first");
            form.Add("LastName", "last");
            form.Add("Email", "someUser@g4s.com");
            form.Add("RoleIds", "1,2");

            return form;
        }
    }
}
