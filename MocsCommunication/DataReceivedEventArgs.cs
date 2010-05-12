using System;

namespace MocsCommunication
{
    public class DataReceivedEventArgs : EventArgs
    {
        private readonly CommunicationMessage _message;
        public DataReceivedEventArgs(CommunicationMessage communicationMessage)
        {
            _message = communicationMessage;
        }

        public CommunicationMessage CommunicationMessage
        {
            get
            {
                return _message;
            }
        }
    }
}
