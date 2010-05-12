using System.Media;
using System.Reflection;
using System.Xml;
using MocsMessageInterceptor;

namespace MocSampleMessageInterceptors
{
    public class PlaySoundMessageInterceptor:IMessageInterceptor
    {
        /// <summary>
        /// <Message>
        ///     <MessageType>Info</MessageType>            
        ///     <DateTime>20:12:00</DateTime>
        ///     <TeamId></TeamId>
        ///     <Category>Test</Category>
        ///     <Text>Dit is een bericht</Text>
        /// </Message>            
        /// </summary>
        public void ProcessMessage(string message)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(message);
            string messageType = xmlDocument.SelectSingleNode("Message/MessageType").InnerText;
            
            if (messageType == "Error")
            {
                SoundPlayer soundPlayer = new SoundPlayer(Assembly.GetExecutingAssembly().GetManifestResourceStream("MocSampleMessageInterceptors.ouch.wav"));
                soundPlayer.Play();
            }
            else if (messageType == "Warning")
            {
                SoundPlayer soundPlayer = new SoundPlayer(Assembly.GetExecutingAssembly().GetManifestResourceStream("MocSampleMessageInterceptors.tarzanyell.wav"));
                soundPlayer.Play();
            }
            else if (messageType == "Info")
            {
                SoundPlayer soundPlayer = new SoundPlayer(Assembly.GetExecutingAssembly().GetManifestResourceStream("MocSampleMessageInterceptors.applause2.wav"));
                soundPlayer.Play();
            }
        }
    }
}
