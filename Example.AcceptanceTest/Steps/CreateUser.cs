using Example.AcceptanceTest.StepHelpers;
using Example.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace Example.AcceptanceTest.Steps
{
    [Binding]
    public class CreateCustomer
    {
        [Then(@"I should be redirected to the User admin screen")]
        public void Then_I_Should_Be_Redirected_To_The_User_Admin_Screen()
        {
            Assert.IsTrue(WebBrowser.Current.Title.ToUpperInvariant().Contains("USERS"));
        }

        [AfterFeature("CreateNewUser")]
        public static void Delete_User_Create_In_Feature()
        {
            TestRunHelper.Session.BeginTransaction();
            var repo = new AccountRepository(TestRunHelper.Session);

            var users = repo.GetBy(x => x.UserName == "someuser");

            if (users != null)
            {
                foreach (var user in users)
                {
                    repo.Delete(user);
                }
            }

            TestRunHelper.Session.Transaction.Commit();
        }
    }
}