using System;

namespace MocsCommunication
{
    public enum LogLevel
    {
        None,
        Info,
        Full,
    }

    public interface ILoggerService
    {
        void Log(LogLevel logLevel, string logFileName, string message);
        void LogMessage(string logFileName, string message);
        void LogException(string logFileName, string message);
        void LogException(string logFileName, Exception exception);
    }
}
