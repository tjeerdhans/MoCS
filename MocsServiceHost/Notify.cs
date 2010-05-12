using System;
using System.ServiceModel;
using System.Text;
using System.Configuration;
using MocsCommunication;

namespace MocsServiceHost
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Notify : INotify, IDisposable
    {
        private ServerCommunicationService _server;
        private bool _useLogging = true;

        public Notify()
        {
            StartServerCommunication();
        }

        private void StartServerCommunication()
        {
            _server = new ServerCommunicationService();

            if (_useLogging)
            {
                ILoggerService loggerService = new LoggerService();
                _server.CurrentLogLevel = LogLevel.Info;
                _server.LogFileName = @"c:\server.txt";
                _server.LoggerService = loggerService;
            }

            _server.MulticastIPAddress = ConfigurationManager.AppSettings["MulticastIPAddress"];
            _server.MulticastPort = Int32.Parse(ConfigurationManager.AppSettings["MulticastPort"]);
            _server.ServerPort = Int32.Parse(ConfigurationManager.AppSettings["ServerPort"]);
            _server.DataReceived += new EventHandler<DataReceivedEventArgs>(_server_DataReceived);
            _server.Start();
        }

        void _server_DataReceived(object sender, DataReceivedEventArgs e)
        {
            CommunicationMessage message = new CommunicationMessage(e.CommunicationMessage.Data);
            message.SetForwardFlag();
            message.IsMulticast =! e.CommunicationMessage.IsMulticast;
            _server.Enqueue(message);            
        }

        public void NotifyAll(MessageType messageType, DateTime dateTime, string teamId, string category, string text)
        {            
            StringBuilder sb = new StringBuilder();
            sb.Append("<Message><MessageType>");
            sb.Append(messageType.ToString());
            sb.Append("</MessageType>");
            sb.Append("<DateTime>");
            sb.Append(dateTime.ToString("HH:mm:ss"));
            sb.Append("</DateTime>");
            sb.Append("<TeamId>");
            sb.Append(teamId);
            sb.Append("</TeamId>");
            sb.Append("<Category>");
            sb.Append(category);
            sb.Append("</Category><Text>");
            sb.Append(text + "</Text><Forward>1</Forward></Message>");
            CommunicationMessage message = new CommunicationMessage(sb.ToString());
            message.ToAll = true;
            _server.Enqueue(message);
        }

        public void Dispose()
        {
            if (_server != null)
            {
                _server.Stop();
                _server = null;
            }
        }
    }
}
