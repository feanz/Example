using System;
namespace Web.Infrastructure.Logging
{
    interface ILogger
    {
        string LogDebug(string message);
        string LogError(ApplicationException x);
        string LogError(string message);
        string LogFatal(ApplicationException x);
        string LogFatal(string message);
        void LogInfo(string message);
        void LogWarning(string message);
    }
}
