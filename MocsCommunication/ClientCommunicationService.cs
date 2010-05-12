using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.Net.Sockets;
using System.Net;

namespace MocsCommunication
{
    public class ClientCommunicationService
    {
        private Queue _receiveQueue;
        private Queue _sendQueue;
        private Thread _listenWorkerThread;
        private Thread _listenWorkerMulticastThread;
        private Thread _receiveWorkerThread;
        private Thread _sendWorkerThread;
        private bool _useMulticast;

        private int _bufferSize;

        private Socket _socket;
        private int _serverPort;
        private string _serverIPAddress;

        private bool _isStarted;
        private ILoggerService _loggerService;
        private string _logFileName;
        private LogLevel _logLevel;

        private string _multicastIPAddress;
        private int _multicastPort;
        private Socket _multicastSocket;

        private readonly object _lockObject = new object(); // used for closing the socket correctly


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

        public string ServerIPAddress
        {
            set { _serverIPAddress = value; }
            get { return _serverIPAddress; }
        }

        public bool UseMulticast
        {
            set { _useMulticast = value; }
            get { return _useMulticast; }
        }

        public int ServerPort
        {
            set { _serverPort = value; }
            get { return _serverPort; }
        }

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

        public event EventHandler<SocketExceptionEventArgs> ReceiveError;
        public event EventHandler<SocketExceptionEventArgs> SendError;
        public event EventHandler<DataReceivedEventArgs> DataReceived;

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

                if (_useMulticast)
                {
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
                }
                else
                {
                    Log(LogLevel.Info, "Begin connecting to                        " + _serverIPAddress + ":" + _serverPort);
                    IPAddress ipAddressToConnect = IPAddress.Parse(_serverIPAddress);
                    IPEndPoint ipEndPoint = new IPEndPoint(ipAddressToConnect, _serverPort);
                    _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    _socket.Connect(ipEndPoint);

                    Log(LogLevel.Info, "End connecting to                          " + _serverIPAddress + ":" + _serverPort);
                }

                Log(LogLevel.Info, "Begin starting Worker Threads");
                StartProcessing();
                Log(LogLevel.Info, "End starting Worker Threads");
                Log(LogLevel.Info, "End starting Communication Service");
            }
            catch (Exception exception)
            {
                LogException(exception);
                Stop();
                throw; // exception;
            }
        }

        public void Stop()
        {
            if (!_isStarted)
            {
                return;
            }
            _isStarted = false;
            Log(LogLevel.Info, "Begin stopping Communication Service");
            CloseSocket(ref _socket);
            CloseSocket(ref _multicastSocket);
            CleanUpWorkerThreads();
            CleanUpQueues();
            Log(LogLevel.Info, "End stopping Communication Service");
        }

        public bool IsStarted
        {
            get
            {
                return _isStarted;
            }
        }

        public void Enqueue(CommunicationMessage message)
        {
            if (_isStarted && message != null && message.Data != null)
            {
                lock (_sendQueue.SyncRoot)
                {
                    _sendQueue.Enqueue(message);
                }
                Log(LogLevel.Info, "Enqueued message.                          Length: " + message.Data.Length.ToString().PadLeft(5));
            }
        }

        private void StartProcessing()
        {
            _receiveQueue = new Queue();
            _sendQueue = new Queue();

            _receiveWorkerThread = new Thread(ProcessReceiveQueue);
            _receiveWorkerThread.Name = "ProcessReceiveQueue";
            _receiveWorkerThread.Start();

            _sendWorkerThread = new Thread(ProcessSendQueue);
            _sendWorkerThread.Name = "ProcessSendQueue";
            _sendWorkerThread.Start();

            if (_useMulticast)
            {
                _listenWorkerMulticastThread = new Thread(StartListeningMulticast);
                _listenWorkerMulticastThread.Name = "StartListeningMulticast";
                _listenWorkerMulticastThread.Start();
            }
            else
            {
                _listenWorkerThread = new Thread(StartListening);
                _listenWorkerThread.Name = "StartListening";
                _listenWorkerThread.Start();
            }
        }

        private void StartListening()
        {
            Log(LogLevel.Info, "Listen thread started");
            string savedData = string.Empty;
            byte[] buffer = new byte[_bufferSize];
            while (_isStarted && _socket != null && _socket.Connected)
            {
                try
                {
                    int received = _socket.Receive(buffer);
                    if (received == 0)
                    {
                        // Server disconnected
                        CloseSocket(ref _socket);
                        if (_isStarted)
                        {
                            LogException("The remote server closed the connection");
                            OnReceiveError(null);
                        }
                    }
                    else
                    {
                        Encoding encoding = Encoding.UTF8;
                        string data = encoding.GetString(buffer, 0, received);

                        if (_logLevel >= LogLevel.Full)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("Data received.                             Length: " + data.Length.ToString().PadLeft(5) + " (" + received.ToString().PadLeft(5) + " bytes)");
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
                            Log(LogLevel.Info, "Data received.                             Length: " + data.Length.ToString().PadLeft(5) + " (" + received.ToString().PadLeft(5) + " bytes)");
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
                                _receiveQueue.Enqueue(new CommunicationMessage(completeMessage));
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
            Log(LogLevel.Info, "Listen thread stopped");
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
                                _receiveQueue.Enqueue(new CommunicationMessage(completeMessage));
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
                        Log(LogLevel.Info, "Dequeued message.                          Length: " + message.Data.Length.ToString().PadLeft(5));
                        OnDataReceived(message);
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
                try
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

                        Log(LogLevel.Info, "Begin sending data.                        Length: " + message.Data.Length.ToString().PadLeft(5));
                        char[] dataToSend = message.Data.ToCharArray();

                        Encoding encoding = Encoding.UTF8;
                        byte[] buffer = encoding.GetBytes(dataToSend, 0, dataToSend.Length);

                        if (_logLevel >= LogLevel.Full)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("Data to send.                              Length: " + dataToSend.Length.ToString().PadLeft(5) + " (" + buffer.Length.ToString().PadLeft(5) + " bytes)");
                            sb.Append(Environment.NewLine);
                            sb.Append("[begin]");
                            sb.Append(Environment.NewLine);
                            sb.Append(message.Data);
                            sb.Append(Environment.NewLine);
                            sb.Append("[end]");
                            Log(LogLevel.Full, sb.ToString());
                        }

                        if (_useMulticast)
                        {                            
                            if (remoteIPEndPoint != null)
                            {
                                Log(LogLevel.Info, "Sending data via multicast");
                                _multicastSocket.SendTo(buffer, remoteIPEndPoint);
                                Log(LogLevel.Info, "End sending data                   Length: " + dataToSend.Length.ToString().PadLeft(5) + " (" + buffer.Length.ToString().PadLeft(5) + " bytes) via Multicast");
                            }
                        }
                        else
                        {
                            
                            if (_socket != null && _socket.Connected)
                            {
                                Log(LogLevel.Info, "Sending data via socket");
                                _socket.Send(buffer);
                                Log(LogLevel.Info, "End sending data.                          Length: " + dataToSend.Length.ToString().PadLeft(5) + " (" + buffer.Length.ToString().PadLeft(5) + " bytes)");
                            }
                        }                       
                    }
                }
                catch (SocketException socketException)
                {
                    LogException(socketException);
                    OnSendError(socketException);
                }
                catch (Exception exception)
                {
                    LogException(exception);
                }
                Thread.Sleep(10);
            }
            Log(LogLevel.Info, "Send queue stopped");
        }

        private void OnDataReceived(CommunicationMessage message)
        {
            if (DataReceived != null)
            {
                DataReceived(this, new DataReceivedEventArgs(message));
            }
        }

        private void OnSendError(SocketException socketException)
        {
            if (SendError != null)
            {
                SendError.BeginInvoke(this, new SocketExceptionEventArgs(socketException), null, null);
            }
        }

        private void OnReceiveError(SocketException socketException)
        {
            if (ReceiveError != null)
            {
                ReceiveError.BeginInvoke(this, new SocketExceptionEventArgs(socketException), null, null);
            }
        }

        private void CleanUpWorkerThreads()
        {
            if (_receiveWorkerThread != null)
            {
                CleanThread(ref _receiveWorkerThread);
            }
            if (_sendWorkerThread != null)
            {
                CleanThread(ref _sendWorkerThread);
            }
            if (_listenWorkerThread != null)
            {
                CleanThread(ref _listenWorkerThread);
            }
            if (_listenWorkerMulticastThread != null)
            {
                CleanThread(ref _listenWorkerMulticastThread);
            }
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
                catch (SystemException) { }
            }
            thread = null;
        }

        private void CleanUpQueues()
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
                catch (SystemException)
                {
                    // Do not remove
                }
                finally
                {
                    socket.Close();
                    socket = null;
                }
            }
        }

        private void Log(LogLevel logLevel, string text)
        {
            if (_loggerService != null && _logLevel >= logLevel)
            {
                _loggerService.Log(logLevel, _logFileName, text);
            }
        }

        private void LogException(Exception exception)
        {
            if (_loggerService != null)
            {
                _loggerService.LogException(_logFileName, exception);
            }
        }

        private void LogException(string text)
        {
            if (_loggerService != null)
            {
                _loggerService.LogException(_logFileName, text);
            }
        }
    }
}
