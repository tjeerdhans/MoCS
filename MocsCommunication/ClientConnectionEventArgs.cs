using System;
using System.Net;

namespace MocsCommunication
{
    public class ClientConnectionEventArgs : EventArgs
    {
        private readonly IPEndPoint _ipEndPoint;

        public ClientConnectionEventArgs(IPEndPoint endPoint)
        {
            _ipEndPoint = endPoint;
        }

        public IPEndPoint EndPoint
        {
            get { return _ipEndPoint; }
        }
    }
}
