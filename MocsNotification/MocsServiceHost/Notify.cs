using System;
using System.ServiceModel;
using System.Text;
using System.Configuration;
using MocsCommunication;
using System.ServiceModel.Configuration;
using System.Collections.Generic;
using System.Collections;

namespace MocsServiceHost
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Notify : INotify, IDisposable
    {
        private ServerCommunicationService _server;
        private bool _useLogging = false;
        private Queue _messageQueue;

        public Notify()
        {
            _messageQueue = new Queue();
            StartServerCommunication();
        }

        private void AddToMessageList(CommunicationMessage message)
        {
            lock (_messageQueue.SyncRoot)
            {
                if (_messageQueue.Count > 5000)
                {
                    _messageQueue.Clear();
                }
                _messageQueue.Enqueue(message);
            }
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
            e.CommunicationMessage.SetForwardFlag();
            if (e.CommunicationMessage.IsRequestForMessageList())
            {
                string teamId = e.CommunicationMessage.GetTeamId();
                SendMessageList(teamId, e.CommunicationMessage.IsMulticast);
            }
            else
            {
                AddToMessageList(e.CommunicationMessage);
                e.CommunicationMessage.IsMulticast = !e.CommunicationMessage.IsMulticast;
                _server.Enqueue(e.CommunicationMessage);
            }
        }

        private void SendMessageList(string teamId, bool useMulticast)
        {
            object[] messages;
            lock (_messageQueue.SyncRoot)
            {
                messages = _messageQueue.ToArray();
            }
            List<CommunicationMessage> messagesToSend = new List<CommunicationMessage>();
            int numberSend = 0;
            for (int i = messages.Length - 1; i >= 0; i--)
            {
                CommunicationMessage message = (CommunicationMessage)messages[i];
                if (string.IsNullOrEmpty(message.GetTeamId()) || message.GetTeamId().Equals(teamId, StringComparison.OrdinalIgnoreCase))
                {
                    CommunicationMessage newMessage = new CommunicationMessage(message.Data);
                    newMessage.SetTeamId(teamId);
                    newMessage.IsMulticast = useMulticast;
                    newMessage.SetSynchronizedFlag();
                    messagesToSend.Add(newMessage);
                    numberSend++;
                    if (numberSend >= 50)
                    {
                        break;
                    }
                }
            }
            for (int i = messagesToSend.Count-1; i >= 0 ; i--)
            {
                _server.Enqueue(messagesToSend[i]);
            }
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
            sb.Append(System.Security.SecurityElement.Escape(text) + "</Text><Forward>1</Forward><Synchronized>0</Synchronized></Message>");
            CommunicationMessage message = new CommunicationMessage(sb.ToString());
            AddToMessageList(message);
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
            _messageQueue.Clear();
        }
    }
}
