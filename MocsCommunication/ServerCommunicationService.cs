using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MocsCommunication
{
    public class ServerCommunicationService
    {
        private ILoggerService _loggerService;
        private string _logFileName;
        private LogLevel _logLevel;
        private bool _isStarted;

        private int _serverPort;
        private int _bufferSize;

        private Socket _socket;
        private List<Socket> _socketList;
        private Thread _receiveWorkerThread;
        private Thread _sendWorkerThread;
        private Thread _listenWorkerMulticastThread;
        private Queue _receiveQueue;
        private Queue _sendQueue;

        private string _multicastIPAddress;
        private int _multicastPort;
        private Socket _multicastSocket;

        private readonly object _lockObject = new object(); // used for closing the socket correctly        

        public ILoggerService LoggerService
        {
            set { _loggerService = value; }
            get { return _loggerService; }
        }

        public string LogFileName
        {
            set { _logFileName = value; }
            get { return _logFileName; }
        }

        public LogLevel CurrentLogLevel
        {
            set { _logLevel = value; }
            get { return _logLevel; }
        }

        public string MulticastIPAddress
        {
            set { _multicastIPAddress = value; }
            get { return _multicastIPAddress; }
        }

        public int MulticastPort
        {
            set { _multicastPort = value; }
            get { return _multicastPort; }
        }

        public int ServerPort
        {
            set { _serverPort = value; }
            get { return _serverPort; }
        }

        public event EventHandler<DataReceivedEventArgs> DataReceived;
        public event EventHandler<SocketExceptionEventArgs> ReceiveError;
        public event EventHandler<ClientConnectionEventArgs> ClientConnected;
        public event EventHandler<ClientConnectionEventArgs> ClientDisconnected;

        public void Start()
        {
            if (_isStarted)
            {
                return;
            }
            _isStarted = true;
            try
            {
                Log(LogLevel.Info, "Begin starting Communication Service");
                _bufferSize = 8192;

                _receiveQueue = new Queue();
                _sendQueue = new Queue();
                _socketList = new List<Socket>();

                Log(LogLevel.Info, "Begin socket server start          port " + _serverPort);
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, _serverPort);
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Bind(ipEndPoint);
                _socket.Listen(4);
                Log(LogLevel.Info, "End socket server start            port " + _serverPort);

                Log(LogLevel.Info, "Begin opening Multicast Socket at          " + _multicastIPAddress + ":" + _multicastPort);
                _multicastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                _multicastSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 5);
                _multicastSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                EndPoint multicastEndPoint = new IPEndPoint(IPAddress.Any, _multicastPort);
                _multicastSocket.Bind(multicastEndPoint);
                Log(LogLevel.Info, "Setting multicast ReceiveBufferSize to     " + _bufferSize);
                _multicastSocket.ReceiveBufferSize = _bufferSize;

                MulticastOption multicastOption = new MulticastOption(IPAddress.Parse(_multicastIPAddress), IPAddress.Any);
                _multicastSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, multicastOption);
                Log(LogLevel.Info, "End opening Multicast Socket at            " + _multicastIPAddress + ":" + _multicastPort);

                Log(LogLevel.Info, "Begin starting Worker Threads");
                _receiveWorkerThread = new Thread(ProcessReceiveQueue);
                _receiveWorkerThread.Name = "ProcessReceiveQueue";
                _receiveWorkerThread.Start();

                _sendWorkerThread = new Thread(ProcessSendQueue);
                _sendWorkerThread.Name = "ProcessSendQueue";
                _sendWorkerThread.Start();

                _listenWorkerMulticastThread = new Thread(StartListeningMulticast);
                _listenWorkerMulticastThread.Name = "StartListeningMulticast";
                _listenWorkerMulticastThread.Start();


                Log(LogLevel.Info, "End starting Worker Threads");
                _socket.BeginAccept(new AsyncCallback(OnClientConnect), null);
                Log(LogLevel.Info, "End starting Communication Service");
            }
            catch (Exception exception)
            {
                LogException(exception);
                Stop();
                throw; // exception;
            }
        }

        private void StartListeningMulticast()
        {
            Log(LogLevel.Info, "Listen thread multicast started");
            string savedData = string.Empty;

            while (_isStarted)
            {
                try
                {
                    int nrOfBytesAvailable = _multicastSocket.Available;
                    if (nrOfBytesAvailable > 0)
                    {
                        byte[] buffer = new byte[nrOfBytesAvailable];
                        int received = _multicastSocket.Receive(buffer);

                        if (_logLevel >= LogLevel.Full)
                        {
                            Log(LogLevel.Info, "Nr of bytes available: " + nrOfBytesAvailable + "; Nr of bytes for current datagram: " + received);
                        }

                        Encoding encoding = Encoding.UTF8;
                        string data = encoding.GetString(buffer, 0, received);

                        if (_logLevel >= LogLevel.Full)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("Multicast Data received.                   Length: " + data.Length.ToString().PadLeft(5) + " (" + received.ToString().PadLeft(5) + " bytes)");
                            sb.Append(Environment.NewLine);
                            sb.Append("[begin]");
                            sb.Append(Environment.NewLine);
                            sb.Append(data);
                            sb.Append(Environment.NewLine);
                            sb.Append("[end]");
                            Log(LogLevel.Full, sb.ToString());
                        }
                        else
                        {
                            Log(LogLevel.Info, "Multicast Data received.                   Length: " + data.Length.ToString().PadLeft(5) + " (" + received.ToString().PadLeft(5) + " bytes)");
                        }

                        savedData += data;

                        const string messageEnd = "</Message>";
                        // do we have a message?
                        int indexOfEOM = savedData.IndexOf(messageEnd);
                        while (indexOfEOM != -1)
                        {
                            int messageLength = indexOfEOM + messageEnd.Length;
                            lock (_receiveQueue.SyncRoot)
                            {
                                string completeMessage = savedData.Substring(0, messageLength);
                                CommunicationMessage message = new CommunicationMessage(completeMessage, true);
                                if (!message.IsForward())
                                {
                                    _receiveQueue.Enqueue(message);
                                }
                            }
                            if (savedData.Length > messageLength)
                            {
                                savedData = savedData.Substring(messageLength);
                                indexOfEOM = savedData.IndexOf(messageEnd);
                            }
                            else
                            {
                                savedData = string.Empty;
                                break;
                            }
                        }
                    }
                }
                catch (SocketException socketException)
                {
                    LogException(socketException);
                    OnReceiveError(socketException);
                }
                catch (Exception exception)
                {
                    LogException(exception);
                }
                Thread.Sleep(10);
            }
            Log(LogLevel.Info, "Listen thread multicast stopped");
        }

        public void Stop()
        {
            if (!_isStarted)
            {
                return;
            }
            _isStarted = false;
            Log(LogLevel.Info, "Begin stopping Server Communication Service");
            CleanUpWorkerThreads();
            CleanupQueues();
            CloseSocket();
            Log(LogLevel.Info, "End stopping Server Communication Service");
        }

        public bool IsStarted
        {
            get { return _isStarted; }
        }

        public void Enqueue(CommunicationMessage message)
        {
            if (_isStarted && message != null && message.Data != null)
            {
                lock (_sendQueue.SyncRoot)
                {
                    _sendQueue.Enqueue(message);
                }
                Log(LogLevel.Info, "Enqueued message.                  Length: " + message.Data.Length.ToString().PadLeft(5));
            }
        }


        private void StartListening(object stateInfo)
        {
            string savedData = string.Empty;
            Socket socket = (Socket)stateInfo;
            IPEndPoint remoteEndPoint = (IPEndPoint)socket.RemoteEndPoint;
            Log(LogLevel.Info, "Start listening on                 " + remoteEndPoint);
            byte[] buffer = new byte[_bufferSize];
            while (_isStarted && socket != null && socket.Connected)
            {
                try
                {
                    int received = socket.Receive(buffer);
                    if (received == 0)
                    {
                        // Client disconnected
                        if (_isStarted)
                        {
                            _socketList.Remove(socket);
                            Log(LogLevel.Info, "The client closed the connection");
                        }
                        CloseSocket(ref socket);
                    }
                    else
                    {
                        IPEndPoint ipEndPoint = socket.RemoteEndPoint as IPEndPoint;
                        Encoding encoding = Encoding.UTF8;
                        string data = encoding.GetString(buffer, 0, received);
                        if (_logLevel >= LogLevel.Full)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("Data received                      Length: " + data.Length.ToString().PadLeft(5) + " (" + received.ToString().PadLeft(5) + " bytes) from: " + ipEndPoint.Address);

                            sb.Append(Environment.NewLine);
                            sb.Append("[begin]");
                            sb.Append(Environment.NewLine);
                            sb.Append(data);
                            sb.Append(Environment.NewLine);
                            sb.Append("[end]");
                            Log(LogLevel.Full, sb.ToString());
                        }
                        else
                        {
                            Log(LogLevel.Info, "Data received                      Length: " + data.Length.ToString().PadLeft(5) + " (" + received.ToString().PadLeft(5) + " bytes) from: " + ipEndPoint.Address);
                        }

                        savedData += data;

                        const string messageEnd = "</Message>";
                        // do we have a complete message?                        
                        int indexOfEOM = savedData.IndexOf(messageEnd);
                        while (indexOfEOM != -1)
                        {
                            int messageLength = indexOfEOM + messageEnd.Length;
                            lock (_receiveQueue.SyncRoot)
                            {
                                string completeMessage = savedData.Substring(0, messageLength);
                                _receiveQueue.Enqueue(new CommunicationMessage(completeMessage, false));
                            }
                            if (savedData.Length > messageLength)               // received (the start of) another message
                            {
                                savedData = savedData.Substring(messageLength);
                                indexOfEOM = savedData.IndexOf(messageEnd);
                            }
                            else
                            {
                                savedData = string.Empty;
                                break;
                            }
                        }
                    }
                }
                catch (SocketException socketException)
                {
                    LogException(socketException);
                }
                catch (Exception exception)
                {
                    LogException(exception);
                }
                Thread.Sleep(10);
            }
            if (ClientDisconnected != null)
            {
                ClientDisconnected.BeginInvoke(this, new ClientConnectionEventArgs(remoteEndPoint), null, null);
            }
            Log(LogLevel.Info, "Stopped listening on               " + remoteEndPoint);
        }

        private void OnClientConnect(IAsyncResult ar)
        {
            if (!_isStarted)
            {
                return;
            }
            Thread.CurrentThread.Name = "OnClientConnect(Server)";
            Socket socket = _socket.EndAccept(ar);
            _socketList.Add(socket);
            IPEndPoint ipEndPoint = socket.RemoteEndPoint as IPEndPoint;
            if (ipEndPoint != null)
            {
                Log(LogLevel.Info, "Client Connected                   " + ipEndPoint.Address + " port " + ipEndPoint.Port);
                Thread listenWorkerThread = new Thread(StartListening);
                listenWorkerThread.Name = "ListenThread_" + ipEndPoint;
                listenWorkerThread.Start(socket);

                if (ClientConnected != null)
                {
                    ClientConnected.BeginInvoke(this, new ClientConnectionEventArgs(ipEndPoint), null, null);
                }
            }
            _socket.BeginAccept(new AsyncCallback(OnClientConnect), null);
        }

        private void ProcessSendQueue()
        {
            Log(LogLevel.Info, "Send queue started");

            IPEndPoint remoteIPEndPoint = null;
            IPAddress ipAddress = IPAddress.Parse(_multicastIPAddress);
            if (ipAddress != null)
            {
                remoteIPEndPoint = new IPEndPoint(ipAddress, _multicastPort);
            }

            while (_isStarted)
            {
                CommunicationMessage message = null;
                lock (_sendQueue.SyncRoot)
                {
                    if (_isStarted && _sendQueue.Count > 0)
                    {
                        message = _sendQueue.Dequeue() as CommunicationMessage;
                    }
                }
                if (message != null)
                {
                    Log(LogLevel.Info, "Begin sending data.                Length: " + message.Data.Length.ToString().PadLeft(5));
                    char[] dataToSend = message.Data.ToCharArray();
                    Encoding encoding = Encoding.UTF8;
                    byte[] buffer = encoding.GetBytes(dataToSend, 0, dataToSend.Length);

                    if (_logLevel >= LogLevel.Full)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Data to send.                      Length: " + dataToSend.Length.ToString().PadLeft(5) + " (" + buffer.Length.ToString().PadLeft(5) + " bytes)");
                        sb.Append(Environment.NewLine);
                        sb.Append("[begin]");
                        sb.Append(Environment.NewLine);
                        sb.Append(message.Data);
                        sb.Append(Environment.NewLine);
                        sb.Append("[end]");
                        Log(LogLevel.Full, sb.ToString());
                    }
                    else
                    {
                        Log(LogLevel.Info, "Data to send.                      Length: " + dataToSend.Length.ToString().PadLeft(5) + " (" + buffer.Length.ToString().PadLeft(5) + " bytes)");
                    }

                    if (message.ToAll || message.IsMulticast)
                    {
                        if (remoteIPEndPoint != null)
                        {
                            Log(LogLevel.Info, "Sending data via multicast");
                            _multicastSocket.SendTo(buffer, remoteIPEndPoint);
                            Log(LogLevel.Info, "End sending data                   Length: " + dataToSend.Length.ToString().PadLeft(5) + " (" + buffer.Length.ToString().PadLeft(5) + " bytes) via Multicast");
                        }
                    }

                    int i = 0;
                    int nrOfRecipients = 0;         // Just displayed in the logfile
                    while (i < _socketList.Count)
                    {
                        Socket socket = _socketList[i];
                        IPEndPoint ipEndPoint = socket.RemoteEndPoint as IPEndPoint;
                        if (ipEndPoint != null)
                        {
                            try
                            {
                                socket.Send(buffer);
                                nrOfRecipients++;
                            }
                            catch (SocketException)
                            {
                                CloseSocket(ref socket);
                                _socketList.RemoveAt(i);
                                i--;
                                if (ClientDisconnected != null)
                                {
                                    ClientDisconnected.BeginInvoke(this, new ClientConnectionEventArgs(ipEndPoint), null, null);
                                }
                            }
                            i++;
                        }
                    }
                    Log(LogLevel.Info, "End sending data.                  Length: " + dataToSend.Length.ToString().PadLeft(5) + " (" + buffer.Length.ToString().PadLeft(5) + " bytes) to " + nrOfRecipients.ToString().PadLeft(2) + "/" + _socketList.Count + " recipients");

                }
                Thread.Sleep(10);
            }
            Log(LogLevel.Info, "Send queue stopped");
        }

        private void ProcessReceiveQueue()
        {
            Log(LogLevel.Info, "Receive queue started");
            while (_isStarted)
            {
                CommunicationMessage message = null;
                try
                {
                    lock (_receiveQueue.SyncRoot)
                    {
                        if (_isStarted && _receiveQueue.Count > 0)
                        {
                            message = _receiveQueue.Dequeue() as CommunicationMessage;
                        }
                    }
                    if (message != null)
                    {                        
                        Log(LogLevel.Info, "Dequeued message.                  Length: " + message.Data.Length.ToString().PadLeft(5));
                        if (DataReceived != null)
                        {
                            DataReceived(this, new DataReceivedEventArgs(message));
                        }
                    }
                }
                catch (Exception exception)
                {
                    LogException(exception);
                }
                Thread.Sleep(10);
            }
            Log(LogLevel.Info, "Receive queue stopped");
        }

        private void CleanUpWorkerThreads()
        {
            Log(LogLevel.Info, "Begin stopping Worker Threads");
            if (_receiveWorkerThread != null)
            {
                CleanThread(ref _receiveWorkerThread);
            }
            if (_sendWorkerThread != null)
            {
                CleanThread(ref _sendWorkerThread);
            }
            if (_listenWorkerMulticastThread != null)
            {
                CleanThread(ref _listenWorkerMulticastThread);
            }
            Log(LogLevel.Info, "End stopping Worker Threads");
        }

        private static void CleanThread(ref Thread thread)
        {
            int i = 0;
            while (thread.ThreadState != ThreadState.Stopped && i < 10)
            {
                Thread.Sleep(50);
                i++;
            }
            if (thread.ThreadState != ThreadState.Stopped)
            {
                try
                {
                    thread.Abort();
                }
                catch (Exception)
                { }
            }
            thread = null;
        }

        private void CleanupQueues()
        {
            if (_receiveQueue != null)
            {
                _receiveQueue.Clear();
                _receiveQueue = null;
            }
            if (_sendQueue != null)
            {
                _sendQueue.Clear();
                _sendQueue = null;
            }
        }

        private void CloseSocket()
        {
            Log(LogLevel.Info, "Begin closing sockets");
            CloseSocket(ref _multicastSocket);
            CloseSocket(ref _socket);   // by passing the reference, the socket is really released and not just closed
            if (_socketList != null)
            {
                for (int i = 0; i < _socketList.Count; i++)
                {
                    Socket socket = _socketList[i];
                    CloseSocket(ref socket);
                }
                _socketList.Clear();
                _socketList = null;
            }
            Log(LogLevel.Info, "End closing sockets");
        }

        private void CloseSocket(ref Socket socket)
        {
            lock (_lockObject)
            {
                if (socket == null)
                {
                    return;
                }
                try
                {
                    if (socket.Connected)       // Don't try to shutdown a connection that is already closed. It will raise error 10057
                    {
                        socket.Shutdown(SocketShutdown.Send);
                        while (socket.Receive(new byte[0]) > 0) { }
                    }
                }
                catch (SocketException socketException)
                {
                    LogException("CloseSocket");
                    LogException(socketException);
                }
                finally
                {
                    socket.Close();
                    socket = null;
                }
            }
        }

        private void OnReceiveError(SocketException socketException)
        {
            if (ReceiveError != null)
            {
                ReceiveError.BeginInvoke(this, new SocketExceptionEventArgs(socketException), null, null);
            }
        }

        private void Log(LogLevel logLevel, string text)
        {
            if (_loggerService != null && _logLevel >= logLevel)
            {
                _loggerService.Log(logLevel, _logFileName, text);
            }
        }

        private void LogException(string errorText)
        {
            if (_loggerService != null)
            {
                _loggerService.LogException(_logFileName, errorText);
            }
        }

        private void LogException(Exception exception)
        {
            if (_loggerService != null)
            {
                _loggerService.LogException(_logFileName, exception);
            }
        }
    }
}
