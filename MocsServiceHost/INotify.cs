using System;
using System.ServiceModel;

namespace MocsServiceHost
{
    public enum MessageType
    {
        Chat,
        Info,        
        Warning,
        Error
    }

    [ServiceContract]
    public interface INotify
    {
        [OperationContract(IsOneWay=true)]
        void NotifyAll(MessageType messageType, DateTime dateTime, string teamId, string category, string text);
    }
}
