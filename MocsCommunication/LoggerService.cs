using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;

namespace MocsCommunication
{
    public class LoggerService : ILoggerService
    {        
        readonly Object _lockThis = new Object();

        public void Log(LogLevel logLevel, string logFileName, string message)
        {
            string messageText = WrapMessage(logLevel, message);
            LogMessage(logFileName, messageText);
        }

        public void LogMessage(string logFileName, string message)
        {
            lock (_lockThis)
            {
                try
                {
                    string path = Path.GetDirectoryName(logFileName);
                    if (path != string.Empty && !Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (FileStream fileStream = File.Open(logFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(message);
                        fileStream.Write(bytes, 0, bytes.Length);
                        fileStream.Flush();
                    }
                }
                catch (Exception exception)
                {
                    try
                    {
                        const string source = "Logger";
                        if (!EventLog.SourceExists(source))
                        {
                            EventLog.CreateEventSource(source, "Application");
                        }
                        EventLog.WriteEntry(source, string.Format("Error in Logger: {0}, Stacktrace: {1}", exception.Message, exception.StackTrace));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public void LogException(string logFileName, string message)
        {
            string errorText = WrapException(message);
            LogMessage(logFileName, errorText);
        }

        public void LogException(string logFileName, Exception exception)
        {
            string errorText = WrapException(exception);
            LogMessage(logFileName, errorText);
        }

        private static string WrapMessage(LogLevel logLevel, string message)
        {
            StringBuilder messageText = new StringBuilder();
            messageText.Append(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss:fff") + " " + logLevel.ToString().ToUpper().PadRight(6) + message);
            messageText.Append(Environment.NewLine);

            return messageText.ToString();
        }

        private static string WrapException(Exception exception)
        {
            StringBuilder errorText = new StringBuilder();
            errorText.Append(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss:fff") + " ERROR " + Environment.NewLine + Environment.NewLine);
            errorText.Append("Computer name:     " + SystemInformation.ComputerName + Environment.NewLine);
            errorText.Append("User name:         " + SystemInformation.UserName + Environment.NewLine);
            errorText.Append("OS:                " + Environment.OSVersion + Environment.NewLine);
            errorText.Append("Culture:           " + CultureInfo.CurrentCulture.Name + Environment.NewLine);
            errorText.Append("Exception class:   " + exception.GetType() + Environment.NewLine);
            errorText.Append("Exception message: " + GetExceptionStack(exception));
            errorText.Append("Stack Trace:");
            errorText.Append(Environment.NewLine);
            errorText.Append(exception.StackTrace);
            errorText.Append(Environment.NewLine);
            errorText.Append(Environment.NewLine);
            return errorText.ToString();
        }

        private static string WrapException(string message)
        {
            StringBuilder errorText = new StringBuilder();
            errorText.Append(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss:fff") + " ERROR " + message);
            errorText.Append(Environment.NewLine);
            return errorText.ToString();
        }

        private static string GetExceptionStack(Exception exception)
        {
            StringBuilder messageText = new StringBuilder();
            messageText.Append(exception.Message + Environment.NewLine);
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                messageText.Append(Environment.NewLine);
                messageText.Append("Exception class:   " + exception.GetType() + Environment.NewLine);
                messageText.Append("Exception message: " + exception.Message + Environment.NewLine);
            }
            return messageText.ToString();
        }        
    }
}
