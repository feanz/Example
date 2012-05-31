using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;
using WatiN.Core;
using Table = TechTalk.SpecFlow.Table;
using Example.AcceptanceTest.Extensions;

namespace Example.AcceptanceTest.Steps
{
    [Binding]
    public class PageInteraction
    {
        [When("I click the \"(.*)\" link")]
        public void When_I_Click_A_Link_Named(string linkName)
        {
            var link = WebBrowser.Current.Link(Find.ByText(linkName));

            if (!link.Exists)
                Assert.Fail(string.Format("Could not find {0} link on the page", linkName));

            link.Click();
        }

        [When("I click the \"(.*)\" button")]
        public void When_I_Click_A_Button_With_Value(string buttonValue)
        {
            var button = WebBrowser.Current.Button(Find.ByValue(buttonValue));

            if (!button.Exists)
                Assert.Fail(string.Format("Could not find {0} button on the page", buttonValue));

            button.Click();
        }

        [When(@"I enter the following information")]
        public void When_I_Enter_The_Following_Information(Table table)
        {
            foreach (var tableRow in table.Rows)
            {
                var field = WebBrowser.Current.TextField(Find.ByName(tableRow["Field"]));

                if (!field.Exists)
                    Assert.Fail(string.Format("Could not find {0} field on the page", field));

                field.TypeTextQuickly(tableRow["Value"]);
            }
        }

        [Then(@"I should see the following details on the screen:")]
        public void Then_I_Should_See_The_Following_Details_On_The_Screen(Table table)
        {
            foreach (var tableRow in table.Rows)
            {
                var value = tableRow["Value"];

                Assert.IsTrue(WebBrowser.Current.ContainsText(value),
                    string.Format("Could not find text '{0}' on the page", value));
            }
        }
    }
}