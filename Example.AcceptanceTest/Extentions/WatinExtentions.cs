using WatiN.Core;

namespace Example.AcceptanceTest.Extensions
{
    public static class WatiNExtensions
    {
        /// <summary>
        /// Sets text quickly, but does not raise key events or focus/blur events
        /// Source: http://blog.dbtracer.org/2010/08/05/speed-up-typing-text-with-watin/
        /// </summary>
        /// <param name="textField"></param>
        /// <param name="text"></param>
        public static void TypeTextQuickly(this TextField textField, string text)
        {
            textField.SetAttributeValue("value", text);
        }
    }
}