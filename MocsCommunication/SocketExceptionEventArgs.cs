using System;
using System.Net.Sockets;

namespace MocsCommunication
{
    public class SocketExceptionEventArgs : EventArgs
    {
        private readonly SocketException _socketException;

        public SocketExceptionEventArgs(SocketException socketException)
        {
            _socketException = socketException;
        }

        public SocketException SocketException
        {
            get
            {
                return _socketException;
            }
        }
    }
}
