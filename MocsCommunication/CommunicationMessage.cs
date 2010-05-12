using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace MocsCommunication
{
    public class CommunicationMessage
    {
        private string _data;
        private bool _toAll;
        private bool _isMulticast;

        public CommunicationMessage(string data)
        {
            _data = data;
        }


        public CommunicationMessage(string data, bool isMulticast)
        {
            _data = data;
            _isMulticast = isMulticast;
        }   


        public void SetForwardFlag()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(_data);
            XmlNode forwardNode = xmlDocument.SelectSingleNode("Message/Forward");
            if (forwardNode != null)
            {
                forwardNode.InnerText = "1";
                _data = xmlDocument.InnerXml;
            }
        }

        public bool IsForward()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(_data);

            XmlNode forwardNode = xmlDocument.SelectSingleNode("Message/Forward");
            if (forwardNode != null)
            {
                string forward = forwardNode.InnerText;
                return forward == "1";
            }
            return false;
        }

        public bool ToAll
        {
            get { return _toAll;}
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
    }
}
