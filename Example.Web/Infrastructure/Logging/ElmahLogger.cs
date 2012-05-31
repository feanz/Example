using System.Web;
using Elmah;

namespace Web.Infrastructure.Logging
{
    public class ElmahLogger : ILogger
    {

        private ErrorLog _logger;

        public ElmahLogger()
        {
            _logger = ErrorLog.GetDefault(HttpContext.Current);
        }

        public void LogInfo(string message)
        {
            var e = new System.ApplicationException(message);
            this.LogError(e);
        }

        public void LogWarning(string message)
        {
            var e = new System.ApplicationException(message);
            this.LogError(e);
        }

        public string LogDebug(string message)
        {
            var e = new System.ApplicationException(message);
            return this.LogError(e);
        }

        public string LogError(string message)
        {
            var e = new System.ApplicationException(message);
            return this.LogError(e);
        }
        public string LogError(System.ApplicationException x)
        {
            var context = HttpContext.Current;
            return _logger.Log(new Error(x, context));
        }
        public string LogFatal(string message)
        {
            var e = new System.ApplicationException(message);
            return this.LogError(e);
        }
        public string LogFatal(System.ApplicationException x)
        {
            return this.LogError(x);
        }
    }
}
