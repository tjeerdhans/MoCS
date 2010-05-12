using MocsMessageInterceptor;
using System.Xml;

namespace MoCSamBXMessageInterceptors
{
    public class AmbientLightInterceptor : IMessageInterceptor
    {
        /// <summary>
        /// <Message>
        ///     <MessageType>Info</MessageType>            
        ///     <DateTime>20:12:00</DateTime>
        ///     <TeamId></TeamId>
        ///     <Category>Test</Category>
        ///     <Text>Dit is een bericht</Text>
        ///     <Forward>0</Forward>
        ///     <Synchronized>0</Synchronized>
        /// </Message>            
        /// </summary>
        public void ProcessMessage(string message)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(message);
                string messageType = xmlDocument.SelectSingleNode("Message/MessageType").InnerText;
                string category = xmlDocument.SelectSingleNode("Message/Category").InnerText;

                amBXPlayer player = new amBXPlayer();

                switch (messageType.ToLower())
                {
                    case "info":
                        switch (category.ToLower())
                        {
                            case "success":
                                player.PlaySuccess();
                                break;
                            case "submitted":
                                break;
                            case "processing":
                                break;
                        }
                        break;
                    case "error":
                        player.PlayFailure();
                        break;
                }
            }
            catch
            { }
        }
    }
}
