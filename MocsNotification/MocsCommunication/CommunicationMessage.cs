using System.Collections.Generic;
using System.Net;
using System.Xml;
using System;

namespace MocsCommunication
{
    public class CommunicationMessage
    {
        private string _data;
        private bool _toAll;
        private bool _isMulticast;
        private XmlDocument _xmlDocument;

        public CommunicationMessage(string data):this(data,false)
        {
            _data = data;
        }

        public CommunicationMessage(string data, bool isMulticast)
        {
            _data = data;
            _isMulticast = isMulticast;
            _xmlDocument = new XmlDocument();
            _xmlDocument.LoadXml(_data);
        }
      
        public void SetForwardFlag()
        {           
            XmlNode forwardNode = _xmlDocument.SelectSingleNode("Message/Forward");
            if (forwardNode != null)
            {
                forwardNode.InnerText = "1";
                _data = _xmlDocument.InnerXml;
            }
        }
       
        public bool IsForward()
        {      
            XmlNode forwardNode = _xmlDocument.SelectSingleNode("Message/Forward");
            if (forwardNode != null)
            {
                string forward = forwardNode.InnerText;
                return forward == "1";
            }
            return false;
        }
      
        public void SetSynchronizedFlag()
        {           
            XmlNode synchronizedNode = _xmlDocument.SelectSingleNode("Message/Synchronized");
            if (synchronizedNode != null)
            {
                synchronizedNode.InnerText = "1";
                _data = _xmlDocument.InnerXml;
            }
        }

        public bool IsSynchronized()
        {
            XmlNode synchronizedNode = _xmlDocument.SelectSingleNode("Message/Synchronized");
            if (synchronizedNode != null)
            {
                string forward = synchronizedNode.InnerText;
                return forward == "1";
            }
            return false;
        }
        
        public bool ToAll
        {
            get { return _toAll; }
            set { _toAll = value; }
        }

        public bool IsMulticast
        {
            get { return _isMulticast; }
            set { _isMulticast = value; }
        }

        public string Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        public bool IsRequestForMessageList()
        {        
            XmlNode categoryNode = _xmlDocument.SelectSingleNode("Message/Category");
            if (categoryNode != null)
            {
                return string.Equals("MessageList", categoryNode.InnerText, StringComparison.OrdinalIgnoreCase);

            }
            return false;
        }

        public string GetTeamId()
        {         
            XmlNode teamIdNode = _xmlDocument.SelectSingleNode("Message/TeamId");
            if (teamIdNode != null)
            {
                return teamIdNode.InnerText;
            }
            return string.Empty;
        }

        public void SetTeamId(string teamId)
        {          
            XmlNode teamIdNode = _xmlDocument.SelectSingleNode("Message/TeamId");
            if (teamIdNode != null)
            {
                teamIdNode.InnerText = teamId;
                _data = _xmlDocument.InnerXml;
            }
        }
    }
}
