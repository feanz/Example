using TechTalk.SpecFlow;
using WatiN.Core;

namespace Example.AcceptanceTest
{
    public static class WebBrowser
    {
        public static IE Current
        {
            get
            {
                if (!ScenarioContext.Current.ContainsKey("browser"))
                    ScenarioContext.Current["browser"] = new IE();
                return ScenarioContext.Current["browser"] as IE;
            }
        }
    }
}
