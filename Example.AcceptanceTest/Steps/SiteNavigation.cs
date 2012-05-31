using TechTalk.SpecFlow;
using System.Security.Principal;
using System.Web.Security;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Example.AcceptanceTest.Steps
{
    [Binding]
    public class SiteNavigation
    {
        [Given(@"I am Authenticated on the site")]
        public void Given_Iam_Authenticated_On_The_Site()
        {
            WebBrowser.Current.GoTo("http://localhost:16387");
            
            if(WebBrowser.Current.Title.Contains("Error"))
                Assert.Fail();
        }

        [Given(@"I on the User admin screen")]
        public void Given_Iam_On_The_User_Admin_Screen()
        {
            WebBrowser.Current.GoTo("http://localhost:16387/Account/Index");
        }
    }
}