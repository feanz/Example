using NHibernate;
using TechTalk.SpecFlow;
using Web.Persistence;
using WatiN.Core;
using Example.Data;

namespace Example.AcceptanceTest.StepHelpers
{
    [Binding]
    public class TestRunHelper
    {
        public static ISessionFactory Factory = NHibernateConfiguration.BuildSessionFactory();
        public static ISession Session = Factory.OpenSession();
        
        [BeforeTestRun]
        public static void Setup()
        {
            
        }

        [AfterTestRun]
        public static void TearDown()
        {
            if (ScenarioContext.Current.ContainsKey("browser"))
            {
                var browser = ScenarioContext.Current["browser"] as IE;
                browser.Close();
            }
        }
    }
}